// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Beatmaps;
using osu.Game.Rulesets.Typing.Layouts;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osu.Game.Rulesets.Typing.Objects;
using static osu.Game.Rulesets.Typing.Layouts.KeyboardData.KeyboardLayout;

namespace osu.Game.Rulesets.Typing.Mods
{
    // Note: This class contains some copy-pasted code from TaikoModFullRandom, because I'm lazy
    // Note 2: While this mod "technically" works as intended, the code could definitely look better, but a complete
    // refactor would be painful with zero tests in place
    public class TypingModWords : TypingMod, IApplicableToBeatmap, IApplicableToBeatmapConverter
    {
        /// <summary>
        /// Stores the counts of adjusted beats by <see cref="LetterSpacing"/> to the next spot where the word will be placed.
        /// <para/>Meant for word lengths: 1, 3, 5, 7 respectively. This small lookup defines the filling space to
        /// the next calculated beat snap.
        /// </summary>
        private static readonly Dictionary<WordSpacing, List<double>> word_spacing_count_lookup = new Dictionary<WordSpacing, List<double>>
        {
            // Note: This exists only for easier lookup and by default there's always one extra half-beat spacing
            { Mods.WordSpacing.Narrow, new List<double> { 1, 1, 1, 1 } },
            // These values were chosen to always snap to nearest full beat, ensuring __at least__ one 1/1 break
            { Mods.WordSpacing.FullBeat, new List<double> { 3, 5, 3, 5 } },
            // These value were chosen to always snap to nearest Odd Full Beat, think of starting a word on "strong" beat (rhythmically)
            { Mods.WordSpacing.EveryOtherFullBeat, new List<double> { 7, 5, 11, 9 } },
        };

        private const int max_banned_consonants_length = 8;

        public override ModType Type => ModType.Conversion;

        public override LocalisableString Description => "Generates random words from dictionary";

        public override string Acronym => "W";

        public override string ExtendedIconInformation
        {
            get
            {
                string info = DictionarySize.Value switch
                {
                    Mods.DictionarySize.Curated => "C",
                    Mods.DictionarySize.CuratedFullAlt => "CX",
                    Mods.DictionarySize.Basic => "B",
                    Mods.DictionarySize.Advanced => "A",
                    Mods.DictionarySize.Extended => "X",
                    _ => throw new ArgumentOutOfRangeException()
                };

                // Note: none of this is necessary, because there are no matching symbols for this customisation
                switch (WordSpacing.Value)
                {
                    case Mods.WordSpacing.FullBeat:
                        info += "/";
                        break;

                    case Mods.WordSpacing.EveryOtherFullBeat:
                        info += "//";
                        break;
                }

                if (AddBonusSpaceHitObjects.Value)
                    info += "_";

                switch (LetterSpacing.Value)
                {
                    case Mods.LetterSpacing.Narrow:
                        info += "+";
                        break;

                    case Mods.LetterSpacing.Wide:
                        info += "-";
                        break;
                }

                return info;
            }
        }

        public override string Name => "Words";

        public override bool Ranked => false;

        [SettingSource("Letter Spacing", "Halves or Doubles the existing beat length to make letters appear more or less frequent.")]
        public Bindable<LetterSpacing> LetterSpacing { get; } = new Bindable<LetterSpacing>(Mods.LetterSpacing.Default);

        [SettingSource("Word spacing", "Allows adjusting the spacing between words by snapping them to next 1/1 or every other 1/1.")]
        public Bindable<WordSpacing> WordSpacing { get; } = new Bindable<WordSpacing>();

        [SettingSource("Dictionary Size", "\"Curated\" dictionary contains a custom, scored and curated words list from Extended dictionary (OANC). Basic/Advanced/Extended - 300/1250/~2500 words.")]
        public Bindable<DictionarySize> DictionarySize { get; } = new Bindable<DictionarySize>();

        [SettingSource("Force cross-hand on new word", "First character in next word starts on the opposite hand. Disable for regular word generation.")]
        public BindableBool ForceCrossHandOnNewWord { get; } = new BindableBool(true);

        [SettingSource("Add bonus Space key objects", "Generates hit objects between words that act as Space in typing. They grant bonus score, but are not required to hit."
                                                      + " Use with caution, because ignoring them can cause note-locks!")]
        public BindableBool AddBonusSpaceHitObjects { get; } = new BindableBool();

        [SettingSource("Banned consonants", "Skips words containing the set consonants. You can add up to 8 characters.")]
        public Bindable<string> BannedConsonants { get; } = new Bindable<string>(string.Empty);

        [SettingSource("KeyboardLayout", "Primarily used for difficulty calculation. Will also visually affect the Key Timing Distribution, changing the key positions on the matrix.")]
        public Bindable<KeyboardLayoutType> KeyboardLayout { get; } = new Bindable<KeyboardLayoutType>();

        public KeyboardLayout SelectedKeyboardLayout { get; private set; } = KEYBOARD_LAYOUTS[KeyboardLayoutType.QwertyStaggered];

        private WeightedRandomWordGenerator wordGenerator = null!;
        private readonly RecentlyUsedWords recentlyUsedWords = new RecentlyUsedWords();

        public TypingModWords()
        {
            BannedConsonants.BindValueChanged(OnBannedLettersChanged);
            KeyboardLayout.BindValueChanged(OnKeyboardLayoutChange);
        }

        private TypingBeatmap typingBeatmap = null!;

        /// <summary>
        /// Used to advance the time in the beatmap.
        /// </summary>
        private double currentTime;

        private double startGenerationAt;
        private double endGenerationAt;

        /// <summary>
        /// Base beat division (1/1) for the current timing point. This length may be adjusted by <see cref="LetterSpacing"/>.
        /// </summary>
        private double fullBeat => currentTimingControlPoint.BeatLength * LetterSpacing.Value switch
        {
            Mods.LetterSpacing.Narrow => 0.5,
            Mods.LetterSpacing.Default => 1,
            Mods.LetterSpacing.Wide => 2,
            _ => 1
        };

        private double halfBeat => fullBeat / 2;

        private bool isStillWithinPlayingBounds => currentTime <= endGenerationAt;

        private TimingControlPoint currentTimingControlPoint = null!;
        private TimingControlPoint lastUsedTimingControlPoint = null!;
        private TimingControlPoint timingPointAtCurrentTime => typingBeatmap.ControlPointInfo.TimingPointAt(currentTime);

        private bool hasTimingPointChanged => !currentTimingControlPoint.Equals(lastUsedTimingControlPoint);

        /// <summary>
        /// Used by <see cref="ForceCrossHandOnNewWord"/> to skip the next word if it starts on the same hand.
        /// </summary>
        private Hand? lastHandUsed;

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            // Breaks have to be deleted, because this mod generates new hit objects, and it WILL place them if the original beatmap had breaks
            beatmapConverter.Beatmap.Breaks.Clear();
        }

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            typingBeatmap = (TypingBeatmap)beatmap;

            if (typingBeatmap.HitObjects.Count < 2)
                return;

            initialiseSettings();

            typingBeatmap.HitObjects.Clear();

            string currentWord = generateWord();
            bool isGeneratingFirstWord = true;

            // Since hit objects are not aware of being a part of a word, which form a pattern, we have to keep track of
            // anything that is about to be inserted into the beatmap, assuming there was no Timing Point to interrupt this
            List<TypingHitObject> hitObjectsInWord = new List<TypingHitObject>();

            // Holds the whole word that was previously inserted into the beatmap, which may be removed if there was a
            // Timing Point inserted while the word was being generated. This word (a set of letters to be exact) will
            // be removed if the next Timing Point happens to be placed in a way that the word ends too close to it.
            // It will be removed if we end up with two words being too close to each other, because we want to make sure
            // the words are separated and readable
            List<TypingHitObject> lastInsertedWord = new List<TypingHitObject>();

            // Used only when the Timing Point changes for comparison if the pattern is about to be inserted suddenly
            // with a different BPM than initialised so the previous word can be force-removed
            TypingHitObject? lastHitObject = null;

            while (isStillWithinPlayingBounds)
            {
                using var enumerator = currentWord.GetEnumerator();

                // Because the first two objects are ignored by difficulty calculators, we have to artificially reduce the starting index
                // so we don't start with index of 2 immediately, bumping the strain. This is because some Skills may use
                // the index (the actual, non-zero index) as where the current letter is located in the word. The very
                // first word would end up with having the first letter as index of 3 instead of 1, because DiffCalc only creates
                // DHOs after the second hit object
                int index = isGeneratingFirstWord ? -2 : 0;
                isGeneratingFirstWord = false;
                hitObjectsInWord.Clear();

                // This condition is almost always true and can become false only if the timing point has changed,
                // meaning that the current word will never be inserted and new word will be picked. This is
                // intentional, because the timing change can happen in the middle of the word, causing letters
                // to be placed closer, resulting in unreadable patterns
                bool canAddWord = true;

                // Only used to remove the already inserted word into the beatmap if the new word happened
                // to be too close __after__ a timing point change. The reason for doing it this way is that it's better
                // to immediately start inserting a new word at the timing point, sacrificing the previous word even
                // if it introduces a longer gap
                bool shouldRemoveLastUsedWord = false;

                while (enumerator.MoveNext())
                {
                    index++;

                    TypingHitObject? hit = createRandomHitObject(enumerator.Current);

                    // We have to purposely force-break here, because this happens where we fall out of the playing bounds,
                    // and we don't want to generate the remaining letters of this word. This was done to match exactly
                    // when the beatmap ends, even if the last resulting word happens to be split
                    if (hit == null)
                        break;

                    hit.IndexInWord = index;
                    hit.WordLength = currentWord.Length;

                    // Remove the entire word if the timing change happened split the word, meaning that the change
                    // lands in-between the letters and common divisors (in this case it's below computed half-beat)
                    if (hasTimingPointChanged)
                    {
                        // The new word can be too close, this will force the removal of the last inserted word
                        if (index == 1 && lastHitObject != null && hit.StartTime - lastHitObject.StartTime < halfBeat)
                            shouldRemoveLastUsedWord = true;

                        // Skip this word and abort, which will start placing a new word at this point in time
                        canAddWord = false;
                        lastHitObject = null;

                        break;
                    }

                    hitObjectsInWord.Add(hit);
                    lastHitObject = hit;

                    advanceTime(halfBeat);
                }

                // This will only happen if the timing point has changed and new word was too close
                if (shouldRemoveLastUsedWord)
                {
                    foreach (var o in lastInsertedWord)
                        typingBeatmap.HitObjects.Remove(o);
                }

                if (canAddWord)
                {
                    // The reason for another bounds check is that the last Space would still be placed, but
                    // we don't want to add anything extra outside the playing bounds
                    if (AddBonusSpaceHitObjects.Value && isStillWithinPlayingBounds)
                    {
                        var space = new SpaceHitObject { StartTime = currentTime };

                        space.ApplyDefaults(typingBeatmap.ControlPointInfo, typingBeatmap.Difficulty);

                        hitObjectsInWord.Add(space);
                    }

                    typingBeatmap.HitObjects.AddRange(hitObjectsInWord);

                    lastInsertedWord.Clear();
                    lastInsertedWord.AddRange(hitObjectsInWord);

                    // For Word Spacing beat calculation we can't count the space as the part of the pattern,
                    // because it would push the word one beat forward, so we have to act as if it never existed
                    if (AddBonusSpaceHitObjects.Value)
                        hitObjectsInWord.RemoveAt(hitObjectsInWord.Count - 1);

                    int gapIndex = (hitObjectsInWord.Count - 1) / 2;
                    double beatsToInsertAsGap = word_spacing_count_lookup[WordSpacing.Value][gapIndex % 4];

                    // Note: this is calculated rather than snapped from timing point, because it may return a wrong snapped time (way forward)...
                    // Small explanation: the whole purpose of this customisation is to "snap" the start of the next word to the closest 1/1 beat,
                    // depending on the selected spacing.
                    // Each word length has its own "filler" defined as a group of beats forming a big spacing so they don't start on
                    // anywhere between the 1/1. This was manually selected as a personal preference
                    double nextSnapTime = beatsToInsertAsGap * halfBeat;

                    advanceTime(nextSnapTime);

                    lastHandUsed = getKeyFromCharacter(currentWord[^1]).physicalKey.Hand;
                }

                currentWord = generateWord();
            }

            typingBeatmap = null!;
        }

        private void initialiseSettings()
        {
            wordGenerator = TypingRuleset.WordDictionaries[DictionarySize.Value];
            WordsRNG = new Random(Seed.Value ??= RNG.Next());

            startGenerationAt = typingBeatmap.HitObjects.First().StartTime;
            endGenerationAt = typingBeatmap.HitObjects.Last().StartTime;
            currentTime = startGenerationAt;

            currentTimingControlPoint = timingPointAtCurrentTime;

            // The very first timing point change check would report `true`, because there was no previously used timing point,
            // so treat the first timing point as the last one used
            lastUsedTimingControlPoint = currentTimingControlPoint;
        }

        private string generateWord()
        {
            // It'd be stupid to assume that it's guaranteed to never cause an infinite loop,
            // but in case someone adds a custom dictionary with a tiny amount of words, and it hangs, that's on them
            while (true)
            {
                string word = wordGenerator.NextWord(WordsRNG);

                if (recentlyUsedWords.WasRecentlyUsed(word))
                    continue;

                // Self note: `if (word.Any(BannedConsonants.Value.Contains))` allocates every word
                if (word.AsSpan().IndexOfAny(BannedConsonants.Value) >= 0)
                    continue;

                if (ForceCrossHandOnNewWord.Value && getKeyFromCharacter(word[0]).physicalKey.Hand == lastHandUsed)
                    continue;

                recentlyUsedWords.Add(word);

                return word;
            }
        }

        private TypingHitObject? createRandomHitObject(char newChar)
        {
            lastUsedTimingControlPoint = (TimingControlPoint)currentTimingControlPoint.DeepClone();

            if (!isStillWithinPlayingBounds)
                return null;

            if (!currentTimingControlPoint.Equals(timingPointAtCurrentTime))
            {
                currentTimingControlPoint = timingPointAtCurrentTime;
                currentTime = currentTimingControlPoint.Time;
            }

            (TypingAction typingAction, PhysicalKey physicalKey) physicalKey = getKeyFromCharacter(newChar);

            TypingHitObject hitObject = new TypingHitObject
            {
                StartTime = currentTime,
                Letter = physicalKey.typingAction,
                CurrentKey = physicalKey.physicalKey,
            };

            hitObject.ApplyDefaults(typingBeatmap.ControlPointInfo, typingBeatmap.Difficulty);

            return hitObject;
        }

        private void advanceTime(double beat) => currentTime += beat;

        private (TypingAction typingAction, PhysicalKey physicalKey) getKeyFromCharacter(char character)
        {
            TypingAction action = LetterToTypingAction(character);
            SelectedKeyboardLayout.TryGetKey(action, out PhysicalKey physicalKey);

            return (action, physicalKey);
        }

        private void OnKeyboardLayoutChange(ValueChangedEvent<KeyboardLayoutType> e) => SelectedKeyboardLayout = KEYBOARD_LAYOUTS[e.NewValue];

        private void OnBannedLettersChanged(ValueChangedEvent<string> e)
        {
            string value = e.NewValue.ToLowerInvariant();
            char[] letters = new HashSet<char>(value.Where(c => char.IsLetter(c) && !"aeiouy".Contains(c))).ToArray();
            string filtered = new string(letters);

            BannedConsonants.Value = filtered.Length > max_banned_consonants_length
                ? filtered[..max_banned_consonants_length]
                : filtered;
        }

        /// <summary>
        /// Used to prevent duplicate words being generated within <see cref="maximum_words_queued"/> window.
        /// </summary>
        private sealed class RecentlyUsedWords
        {
            private const int maximum_words_queued = 8;

            private readonly Queue<string> recentWords = new Queue<string>();

            public bool WasRecentlyUsed(string word) => recentWords.Contains(word);

            public void Add(string word)
            {
                recentWords.Enqueue(word);

                if (recentWords.Count > maximum_words_queued)
                    recentWords.Dequeue();
            }
        }
    }

    public enum LetterSpacing
    {
        // Half of the Default beat length. In short: twice the BPM
        Narrow,

        // Default beat length value that was calculated for the beatmap
        Default,

        // Twice the Default beat length. In short: half the BPM
        Wide,
    }

    public enum WordSpacing
    {
        Narrow,
        FullBeat,
        EveryOtherFullBeat,
    }

    public enum DictionarySize
    {
        Curated,
        CuratedFullAlt,
        Basic,
        Advanced,
        Extended
    }
}
