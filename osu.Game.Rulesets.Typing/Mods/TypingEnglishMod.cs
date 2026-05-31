// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Beatmaps;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Mods
{
    // Note: This class contains code copy-pasted from TaikoModFullRandom, because I'm lazy
    public abstract class TypingEnglishMod : Mod, IApplicableToBeatmap, IApplicableToBeatmapConverter, ITypingDictionaryMod
    {
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
            typeof(TypingModEnglish25K),
            typeof(TypingModEnglish450K),
        }.Except(new[] { GetType() }).ToArray();

        private TypingBeatmap typingBeatmap = null!;
        private readonly List<TypingHitObject?> faultyHitObjectsToRemove = new List<TypingHitObject?>();
        private TypingHitObject? lastHitObjectCreated;

        /// <summary>
        /// Used to advance the time in the beatmap.
        /// </summary>
        private double currentTime;

        private double startGenerationAt;
        private double endGenerationAt;

        /// <summary>
        /// Base beat division for the current timing point (1/1).
        /// </summary>
        private double baseBeatLength;

        private double beatOneHalf => baseBeatLength / 2;
        private double beatOneFourth => baseBeatLength / 4;

        private bool isStillWithinPlayingBounds => currentTime <= endGenerationAt;

        private TimingControlPoint currentTimingControlPoint = null!;
        private TimingControlPoint timingPointAtCurrentTime => typingBeatmap.ControlPointInfo.TimingPointAt(currentTime);

        public void ApplyToBeatmapConverter(IBeatmapConverter beatmapConverter)
        {
            // Breaks have to be deleted, because this mod generates new hit objects, and it WILL place them if the original beatmap had breaks
            beatmapConverter.Beatmap.Breaks.Clear();
        }

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            beatmap.Breaks.Clear();

            typingBeatmap = (TypingBeatmap)beatmap;

            if (typingBeatmap.HitObjects.Count < 2)
                return;

            initialiseSettings();

            typingBeatmap.HitObjects.Clear();

            while (isStillWithinPlayingBounds)
            {
                TypingHitObject? hit = createRandomHitObject();

                if (hit == null)
                    break;

                typingBeatmap.HitObjects.Add(hit);

                advanceTime(baseBeatLength);
            }

            cleanUp();
        }

        private void initialiseSettings()
        {
            startGenerationAt = typingBeatmap.HitObjects.First().StartTime;
            endGenerationAt = typingBeatmap.HitObjects.Last().StartTime;
            currentTime = startGenerationAt;

            currentTimingControlPoint = timingPointAtCurrentTime;
            baseBeatLength = currentTimingControlPoint.BeatLength;
        }

        private TypingHitObject? createRandomHitObject()
        {
            if (!isStillWithinPlayingBounds)
                return null;

            if (!currentTimingControlPoint.Equals(timingPointAtCurrentTime))
            {
                currentTimingControlPoint = timingPointAtCurrentTime;
                currentTime = currentTimingControlPoint.Time;

                if (lastHitObjectCreated != null && currentTime - lastHitObjectCreated.StartTime < beatOneFourth)
                    faultyHitObjectsToRemove.Add(lastHitObjectCreated);

                baseBeatLength = currentTimingControlPoint.BeatLength;
            }

            TypingHitObject hitObject = new TypingHitObject
            {
                StartTime = currentTime,
                Letter = TypingAction.A
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
    }

    public enum DictionarySize
    {
        // For consistency, 0k was preferred over `EnglishSimple
        E0K,
        E1K,
        E5K,
        E10K,
        E25K,
        E450K,
    }
}
