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

namespace osu.Game.Rulesets.Typing.Mods
{
    // Note: This class contains code copy-pasted from TaikoModFullRandom, because I'm lazy
    public class TypingModWords : TypingMod, IApplicableToBeatmap, IApplicableToBeatmapConverter
    {
        private const int max_banned_consonants_length = 8;

        private static readonly Dictionary<KeyboardLayoutType, KeyboardLayout> keyboard_layouts = new Dictionary<KeyboardLayoutType, KeyboardLayout>
        {
            { KeyboardLayoutType.QwertyStaggered, new QwertyStaggeredLayout() },
            { KeyboardLayoutType.QwertyOrtholinear, new QwertyOrtholinearLayout() },
            { KeyboardLayoutType.DvorakStaggered, new DvorakStaggeredLayout() },
            { KeyboardLayoutType.DvorakOrtholinear, new DvorakOrtholinearLayout() },
            { KeyboardLayoutType.ColemakStaggered, new ColemakStaggeredLayout() },
            { KeyboardLayoutType.ColemakOrtholinear, new ColemakOrtholinearLayout() },
            { KeyboardLayoutType.ColemakDhStaggered, new ColemakDhStaggeredLayout() },
            { KeyboardLayoutType.ColemakDhOrtholinear, new ColemakDhOrtholinearLayout() },
        };

        public override ModType Type => ModType.Conversion;
        public override LocalisableString Description => "Generates random words from dictionary";
        public override string Acronym => "ENG";
        public override string Name => "Words";
        public override bool Ranked => false;

        [SettingSource("Dictionary Size",
            "\"Curated\" dictionary contains a custom scored words list from Extended dictionary with extra words from OANC. \"Basic\" - 300 words. Other dictionaries are frequency-sorted words lists.")]
        public Bindable<DictionarySize> DictionarySize { get; } = new Bindable<DictionarySize>();

        [SettingSource("Adjust Beat Length",
            "Halve or Double the existing beat length to make letters appear more or less frequent. \"Full\" will use the current time value of this beatmaps's half beat (1/2). Half: 1/4, Double: 1/1.")]
        public Bindable<BeatLength> AdjustBeatLength { get; } = new Bindable<BeatLength>(BeatLength.Full);

        [SettingSource("Add spacing between words", "Inserts a full beat pause between the words.")]
        public BindableBool AddSpacingBetweenWords { get; } = new BindableBool();

        [SettingSource("Force cross-hand on new word", "First character in next word starts on the opposite hand. Disable for regular word generation.")]
        public BindableBool ForceCrossHandOnNewWord { get; } = new BindableBool(true);

        [SettingSource("Banned consonants", "Skips words containing the set consonants. You can add up to 8 characters.")]
        public Bindable<string> BannedConsonants { get; } = new Bindable<string>(string.Empty);

        [SettingSource("KeyboardLayout", "Primarily used for difficulty calculation. Will also visually affect the Key Timing Distribution, changing the key positions on the matrix.")]
        public Bindable<KeyboardLayoutType> KeyboardLayout { get; } = new Bindable<KeyboardLayoutType>();

        public KeyboardLayout SelectedKeyboardLayout { get; private set; }

        public TypingModWords()
        {
            BannedConsonants.BindValueChanged(OnBannedLettersChanged);
            KeyboardLayout.BindValueChanged(OnKeyboardLayoutChange);

            SelectedKeyboardLayout = keyboard_layouts[KeyboardLayoutType.QwertyStaggered];
        }

        private void OnKeyboardLayoutChange(ValueChangedEvent<KeyboardLayoutType> e) => SelectedKeyboardLayout = keyboard_layouts[e.NewValue];

        private void OnBannedLettersChanged(ValueChangedEvent<string> e)
        {
            string value = e.NewValue.ToLowerInvariant();
            char[] letters = new HashSet<char>(value.Where(c => char.IsLetter(c) && !"aeiouy".Contains(c))).ToArray();
            string filtered = new string(letters);

            BannedConsonants.Value = filtered.Length > max_banned_consonants_length
                ? filtered[..max_banned_consonants_length]
                : filtered;
        }

        private TypingBeatmap typingBeatmap = null!;

        /// <summary>
        /// Used to advance the time in the beatmap.
        /// </summary>
        private double currentTime;

        private double startGenerationAt;
        private double endGenerationAt;

        /// <summary>
        /// Base beat division for the current timing point (1/1). This length may be adjusted by <see cref="AdjustBeatLength"/>.
        /// </summary>
        private double beatFull => currentTimingControlPoint.BeatLength * AdjustBeatLength.Value switch
        {
            BeatLength.Half => 0.5,
            BeatLength.Full => 1,
            BeatLength.Double => 2,
            _ => 1
        };

        private double beatHalf => beatFull / 2;
        private double beatFourth => beatFull / 4;

        private bool isStillWithinPlayingBounds => currentTime <= endGenerationAt;

        private TimingControlPoint currentTimingControlPoint = null!;
        private TimingControlPoint lastUsedTimingControlPoint = null!;
        private TimingControlPoint timingPointAtCurrentTime => typingBeatmap.ControlPointInfo.TimingPointAt(currentTime);

        private bool hasTimingPointChanged => !currentTimingControlPoint.Equals(lastUsedTimingControlPoint);

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

            RankedWordGenerator wordGenerator = TypingRuleset.RankedDictionaries[DictionarySize.Value];
            WordSamplingContext samplingContext = new WordSamplingContext();

            string currentWord = generateWord(wordGenerator, samplingContext);
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
                        if (index == 1 && lastHitObject != null && hit.StartTime - lastHitObject.StartTime < beatHalf)
                            shouldRemoveLastUsedWord = true;

                        // Skip this word and abort, which will start placing a new word at this point in time
                        canAddWord = false;
                        lastHitObject = null;

                        break;
                    }

                    hitObjectsInWord.Add(hit);
                    lastHitObject = hit;

                    advanceTime(beatHalf);
                }

                // This will only happen if the timing point has changed and new word was too close
                if (shouldRemoveLastUsedWord)
                {
                    foreach (var o in lastInsertedWord)
                        typingBeatmap.HitObjects.Remove(o);
                }

                if (canAddWord)
                {
                    typingBeatmap.HitObjects.AddRange(hitObjectsInWord);

                    lastInsertedWord.Clear();
                    lastInsertedWord.AddRange(hitObjectsInWord);

                    // The last spacing is required
                    advanceTime(beatHalf);

                    // A full beat of breathing room allows to reduce the cognitive load, and make the key travel a bit easier
                    if (AddSpacingBetweenWords.Value)
                        advanceTime(beatFull);

                    lastHandUsed = getKeyFromCharacter(currentWord[^1]).physicalKey.Hand;
                }

                currentWord = generateWord(wordGenerator, samplingContext);
            }

            typingBeatmap = null!;
        }

        private void initialiseSettings()
        {
            WordsRNG = new Random(Seed.Value ??= RNG.Next());

            startGenerationAt = typingBeatmap.HitObjects.First().StartTime;
            endGenerationAt = typingBeatmap.HitObjects.Last().StartTime;
            currentTime = startGenerationAt;

            currentTimingControlPoint = timingPointAtCurrentTime;

            // The very first timing point change check would report `true`, because there was no previously used timing point,
            // so treat the first timing point as the last one used
            lastUsedTimingControlPoint = currentTimingControlPoint;
        }

        private string generateWord(RankedWordGenerator generator, WordSamplingContext context)
        {
            while (true)
            {
                string word = generator.NextWord(WordsRNG);

                if (context.WasRecentlyUsed(word))
                    continue;

                if (word.Any(BannedConsonants.Value.Contains))
                    continue;

                if (ForceCrossHandOnNewWord.Value && getKeyFromCharacter(word[0]).physicalKey.Hand == lastHandUsed)
                    continue;

                context.Push(word);

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

        private sealed class WordSamplingContext
        {
            private readonly Queue<string> recentWords = new Queue<string>();
            private const int recent_window = 8;

            public bool WasRecentlyUsed(string word)
                => recentWords.Contains(word);

            public void Push(string word)
            {
                recentWords.Enqueue(word);

                while (recentWords.Count > recent_window)
                    recentWords.Dequeue();
            }

            public void RemoveQueuedWord(string word)
            {
                int count = recentWords.Count;

                for (int i = 0; i < count; i++)
                {
                    string item = recentWords.Dequeue();

                    if (item != word)
                        recentWords.Enqueue(item);
                }
            }
        }
    }

    public enum BeatLength
    {
        Half,
        Full,
        Double,
    }

    public enum DictionarySize
    {
        Curated,
        Basic,
        Advanced,
        Extended
    }
}
