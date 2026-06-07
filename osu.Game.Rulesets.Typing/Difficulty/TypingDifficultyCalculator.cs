// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Difficulty.Skills;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public class TypingDifficultyCalculator : DifficultyCalculator
    {
        private const double difficulty_multiplier = 0.01;
        private const double strain_skill_multiplier = 0.25 * difficulty_multiplier;
        private const double speed_skill_multiplier = 0.05 * difficulty_multiplier;

        private readonly IKeyboardLayout keyboardLayout;

        public override int Version => 20260607;

        public TypingDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap, IKeyboardLayout layout)
            : base(ruleset, beatmap)
        {
            keyboardLayout = layout;
        }

        protected override Mod[] DifficultyAdjustmentMods => new Mod[]
        {
            new TypingModDoubleTime(),
            new TypingModHalfTime(),
        };

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills, double clockRate)
        {
            if (beatmap.HitObjects.Count == 0)
                return new TypingDifficultyAttributes { Mods = mods };

            var strain = skills.OfType<Strain>().Single();
            var speed = skills.OfType<Speed>().Single();

            double strainValue = strain.DifficultyValue();
            double speedValue = speed.DifficultyValue();

            double combinedRating = combinedDifficultyValue(strain, speed);
            double starRating = rescale(combinedRating * 1.4);

            return new TypingDifficultyAttributes
            {
                StarRating = starRating,
                Mods = mods,
                MaxCombo = beatmap.GetMaxCombo(),
                Strain = strainValue,
                Speed = speedValue,
            };
        }

        private double combinedDifficultyValue(Strain strain, Speed speed)
        {
            List<double> peaks = combinePeaks(
                strain.GetCurrentStrainPeaks().ToList(),
                speed.GetCurrentStrainPeaks().ToList()
            );

            if (peaks.Count == 0)
                return 0;

            double difficulty = 0;
            double weight = 1;

            foreach (double peakStrain in peaks.OrderDescending())
            {
                difficulty += peakStrain * weight;
                weight *= 0.9;
            }

            return difficulty;
        }

        private List<double> combinePeaks(List<double> strainPeaks, List<double> speedPeaks)
        {
            int max = strainPeaks.Count;
            var combined = new List<double>(max);

            for (int i = 0; i < max; i++)
            {
                double strain = i < strainPeaks.Count ? strainPeaks[i] : 0;
                double speed = i < speedPeaks.Count ? speedPeaks[i] : 0;

                combined.Add(strain * strain_skill_multiplier + speed * speed_skill_multiplier);
            }

            return combined;
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
        {
            var objects = new List<DifficultyHitObject>();

            for (int i = 2; i < beatmap.HitObjects.Count; i++)
            {
                keyboardLayout.TryGetKey(((TypingHitObject)beatmap.HitObjects[i]).Letter, out PhysicalKey physicalKey);

                objects.Add(
                    new TypingDifficultyHitObject(
                        beatmap.HitObjects[i],
                        beatmap.HitObjects[i - 1],
                        clockRate,
                        objects,
                        objects.Count,
                        physicalKey,
                        beatmap.HitObjects.ElementAtOrDefault(i + 1)
                    )
                );
            }

            // Assign hit object times and pattern index, which both are used by skills
            for (int i = 0; i < objects.Count; i++)
            {
                TypingDifficultyHitObject current = (TypingDifficultyHitObject)objects[i];
                TypingDifficultyHitObject previous = (TypingDifficultyHitObject)current.Previous(0);
                TypingDifficultyHitObject next = (TypingDifficultyHitObject)current.Next(0);

                if (previous == null || next == null)
                    continue;

                double timeFromPrevious = current.StartTime - previous.StartTime;
                double timeToNext = next.StartTime - current.StartTime;

                current.TimeToNext = timeToNext;
                current.TimeFromPrevious = timeFromPrevious;

                if (timeToNext <= timeFromPrevious)
                    current.IndexInPattern = previous.IndexInPattern + 1;
            }

            return objects;
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
        {
            return new Skill[]
            {
                new Strain(mods),
                new Speed(mods)
            };
        }

        private static double rescale(double sr)
        {
            if (sr < 0)
                return sr;

            return 10.43 * Math.Log(sr / 8 + 1);
        }
    }
}
