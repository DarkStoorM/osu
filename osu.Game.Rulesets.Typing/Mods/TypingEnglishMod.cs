// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
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
    public abstract class TypingEnglishMod : TypingMod, IApplicableToBeatmap, IApplicableToBeatmapConverter, ITypingDictionaryMod
    {
        private const int max_banned_letters_length = 5;

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

        public abstract DictionarySize DictionarySize { get; }
        public override ModType Type => ModType.Conversion;
        public override string Acronym => Name;
        public override bool Ranked => false;

        public override Type[] IncompatibleMods => new[]
        {
            typeof(TypingModEnglish0K),
            typeof(TypingModEnglish1K),
            typeof(TypingModEnglish5K),
            typeof(TypingModEnglish10K),
            typeof(TypingModEnglish25K)
        }.Except(new[] { GetType() }).ToArray();

        [SettingSource("Adjust Beat Length", "Makes spacing shorter or longer between the objects. Half = twice as fast, Double = twice as slow")]
        public Bindable<BeatLength> AdjustBeatLength { get; } = new Bindable<BeatLength>(BeatLength.Full);

        [SettingSource("Add spacing between words", "Inserts a full beat pause between the words")]
        public BindableBool AddSpacingBetweenWords { get; } = new BindableBool();

        [SettingSource("Banned letters", "Skips words containing the set letters")]
        public Bindable<string> BannedLetters { get; } = new Bindable<string>(string.Empty);

        [SettingSource("Skip all even length words", "Makes everything land on-beat. Disable this to include even length words, for more off-beat patterns and variety.")]
        public BindableBool SkipEvenLengthWords { get; } = new BindableBool(true);

        [SettingSource("Chance to generate even length words", "Requires disabling the even length word skip")]
        public BindableFloat EvenWordLengthChance { get; } = new BindableFloat(0.1f)
        {
            MinValue = 0.05f,
            MaxValue = 0.5f,
            Precision = 0.01f
        };

        [SettingSource("KeyboardLayout", "Requires disabling the even length word skip")]
        public Bindable<KeyboardLayoutType> KeyboardLayout { get; } = new Bindable<KeyboardLayoutType>(KeyboardLayoutType.QwertyStaggered);

        private KeyboardLayout selectedKeyboardLayout { get; set; }

        protected TypingEnglishMod()
        {
            BannedLetters.BindValueChanged(OnBannedLettersChanged);
            KeyboardLayout.BindValueChanged(OnKeyboardLayoutChange);

            selectedKeyboardLayout = keyboard_layouts[KeyboardLayoutType.QwertyStaggered];
        }

        private void OnKeyboardLayoutChange(ValueChangedEvent<KeyboardLayoutType> e) => selectedKeyboardLayout = keyboard_layouts[e.NewValue];

        private void OnBannedLettersChanged(ValueChangedEvent<string> e)
        {
            string value = e.NewValue;

            if (value.Length > max_banned_letters_length)
                BannedLetters.Value = value[..max_banned_letters_length];
        }

        private TypingBeatmap typingBeatmap = null!;
        private readonly List<TypingHitObject?> faultyHitObjectsToRemove = new List<TypingHitObject?>();
        private TypingHitObject? lastHitObjectCreated;

        /// <summary>
        /// Used to advance the time in the beatmap.
        /// </summary>
        private double currentTime;

        private double startGenerationAt;
        private double endGenerationAt;

        private HashSet<char> bannedLetters = new HashSet<char>();

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

            RankedWordGenerator wordGenerator = TypingRuleset.RankedDictionaries[DictionarySize];
            WordSamplingContext samplingContext = new WordSamplingContext();

            string currentWord = getNextWord(wordGenerator, samplingContext);
            int currentWordLength = currentWord.Length;
            bool hasJoinedEvenWordYet = false;
            bool isGeneratingFirstWord = true;

            while (isStillWithinPlayingBounds)
            {
                using var enumerator = currentWord.GetEnumerator();

                // Because the first two objects are ignored by difficulty calculators, we have to artificially reduce the starting index
                // so we don't start with index of 2 immediately, bumping the strain
                int index = isGeneratingFirstWord ? -2 : 0;
                isGeneratingFirstWord = false;

                while (enumerator.MoveNext())
                {
                    index++;

                    TypingHitObject? hit = createRandomHitObject(enumerator.Current);

                    if (hit == null)
                        break;

                    hit.IndexInWord = index;
                    hit.WordLength = currentWord.Length;

                    // Avoid placing hit objects on the wrong timing point
                    if (hasTimingPointChanged)
                    {
                        typingBeatmap.HitObjects.Add(hit);

                        advanceTime(beatHalf);
                        break;
                    }

                    typingBeatmap.HitObjects.Add(hit);

                    advanceTime(beatHalf);
                }

                // The last spacing is required
                advanceTime(beatHalf);

                // A full beat of breathing room allows to reduce the cognitive load, and make the key travel a bit easier
                if (AddSpacingBetweenWords.Value)
                    advanceTime(beatFull);

                // To retain the rhythm, we have to keep rolling another even number length word
                if (currentWordLength % 2 == 0 && !hasJoinedEvenWordYet)
                {
                    currentWord = getNextWord(wordGenerator, samplingContext, forceEvenLengthWord: true);
                    currentWordLength = currentWord.Length;
                    hasJoinedEvenWordYet = true;
                }
                else
                {
                    // Otherwise, we already joined the two even number length words, roll whatever
                    currentWord = getNextWord(wordGenerator, samplingContext);
                    currentWordLength = currentWord.Length;
                    hasJoinedEvenWordYet = false;
                }
            }

            if (faultyHitObjectsToRemove.Count > 0)
                typingBeatmap.HitObjects.RemoveAll(h => faultyHitObjectsToRemove.Contains(h));

            cleanUp();
        }

        private void initialiseSettings()
        {
            // Important note about the separation:
            // Words are random anyway, so this effectively changes all the beatmap contents in front of the next generated word.
            // This means that normally, with odd-length words we might see a five stars map, but enabling the even-length
            // words will change which words are generated next even on the same seed. So, because calling the RNG alters the
            // state, this means that previously what made the calculations go up, e.g. long words, they might get removed
            // because of this.
            // The separation was necessary, because we don't want to disrupt the RNG cycle for the default odd-length words,
            // so this only results in stitching words together from two sources, eventually pushing the odd-length words outwards.
            // This is the desired effect to not result in confusing information from WordLength skill, which increases the strain.
            OddLengthWordsRNG = new Random(Seed.Value ??= RNG.Next());
            EvenLengthWordsRNG = new Random(Seed.Value ??= RNG.Next());

            // This only exists to ensure the same RNGs are being picked for the given seed
            HelperRNG = new Random(Seed.Value ??= RNG.Next());

            startGenerationAt = typingBeatmap.HitObjects.First().StartTime;
            endGenerationAt = typingBeatmap.HitObjects.Last().StartTime;
            currentTime = startGenerationAt;

            currentTimingControlPoint = timingPointAtCurrentTime;

            bannedLetters = new HashSet<char>(BannedLetters.Value.ToLowerInvariant().Where(char.IsLetter));
        }

        private string getNextWord(RankedWordGenerator generator, WordSamplingContext samplingContext, bool forceEvenLengthWord = false)
        {
            // The bindable option takes precedence over everything, because we have to respect the player's mod customisation
            if (SkipEvenLengthWords.Value)
                return getWord(isEven: false, OddLengthWordsRNG);

            if (forceEvenLengthWord)
                return getWord(isEven: true, EvenLengthWordsRNG);

            // Pick one at random, but from the seeded RNG so we don't end up with a different word sequence
            Random rng = HelperRNG.NextDouble() < 0.5
                ? OddLengthWordsRNG
                : EvenLengthWordsRNG;

            return rng.NextDouble() < EvenWordLengthChance.Value
                ? getWord(isEven: true, EvenLengthWordsRNG)
                : getWord(isEven: false, OddLengthWordsRNG);

            string getWord(bool isEven, Random wordRng)
            {
                string word;
                bool wordInvalid;

                do
                {
                    word = generateWord(generator, samplingContext, wordRng);
                    wordInvalid = isWordInvalid(word);

                    if (wordInvalid && samplingContext.WasRecentlyUsed(word))
                        samplingContext.RemoveQueuedWord(word);
                } while (wordInvalid);

                return word;

                bool isWordInvalid(string testedWord) => testedWord.Length % 2 == 0 != isEven || testedWord.Any(bannedLetters.Contains);
            }
        }

        private string generateWord(RankedWordGenerator generator, WordSamplingContext context, Random wordRng)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                string word = generator.NextWord(wordRng);

                if (context.WasRecentlyUsed(word))
                    continue;

                context.Push(word);

                return word;
            }

            // Always allow at least something
            string fallback = generator.NextWord(wordRng);

            context.Push(fallback);

            return fallback;
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

                if (lastHitObjectCreated != null && currentTime - lastHitObjectCreated.StartTime < beatFourth)
                    faultyHitObjectsToRemove.Add(lastHitObjectCreated);
            }

            TypingAction action = LetterToTypingAction(newChar);
            selectedKeyboardLayout.TryGetKey(action, out PhysicalKey currentPhysicalKey);

            TypingHitObject hitObject = new TypingHitObject
            {
                StartTime = currentTime,
                Letter = action,
                CurrentKey = currentPhysicalKey,
            };

            hitObject.ApplyDefaults(typingBeatmap.ControlPointInfo, typingBeatmap.Difficulty);

            lastHitObjectCreated = hitObject;

            return hitObject;
        }

        private void advanceTime(double beat) => currentTime += beat;

        private void cleanUp()
        {
            typingBeatmap = null!;
            faultyHitObjectsToRemove.Clear();
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
        // For consistency, 0k was preferred over `EnglishSimple
        E0K,
        E1K,
        E5K,
        E10K,
        E25K
    }
}
