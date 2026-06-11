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
        private const double difficulty_multiplier = 1;
        private const double row_switch_skill_multiplier = 0 * difficulty_multiplier;
        private const double retrigger_skill_multiplier = 0 * difficulty_multiplier;
        private const double key_travel_skill_multiplier = 0 * difficulty_multiplier;
        private const double finger_control_skill_multiplier = 0 * difficulty_multiplier;
        private const double speed_skill_multiplier = 0 * difficulty_multiplier;
        private const double word_length_skill_multiplier = 1 * difficulty_multiplier;

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
            // Difficulty calculation will not be available if the word generation mods are not enabled,
            // because this ruleset is meant to be played only with those mods due to some skills
            // relying on letters being a part of a word, which in this context form a pattern
            if (!mods.Any(x => x is TypingEnglishMod) || beatmap.HitObjects.Count == 0)
                return new TypingDifficultyAttributes { Mods = mods };

            var fingerControl = skills.OfType<FingerControl>().Single();
            var keyTravel = skills.OfType<KeyTravel>().Single();
            var retrigger = skills.OfType<Retrigger>().Single();
            var rowSwitch = skills.OfType<RowSwitch>().Single();
            var speed = skills.OfType<Speed>().Single();
            var wordLength = skills.OfType<WordLength>().Single();

            double fingerControlValue = fingerControl.DifficultyValue();
            double keyTravelValue = keyTravel.DifficultyValue();
            double retriggerValue = retrigger.DifficultyValue();
            double rowSwitchValue = rowSwitch.DifficultyValue();
            double speedValue = speed.DifficultyValue();
            double wordLengthValue = wordLength.DifficultyValue();
            double combinedRating = combinedDifficultyValue(fingerControl, keyTravel, retrigger, rowSwitch, speed, wordLength);
            double starRating = rescale(combinedRating * 1.4);

            return new TypingDifficultyAttributes
            {
                StarRating = starRating,
                Mods = mods,
                MaxCombo = beatmap.GetMaxCombo(),
                FingerControl = fingerControlValue,
                KeyTravel = keyTravelValue,
                Retrigger = retriggerValue,
                RowSwitch = rowSwitchValue,
                Speed = speedValue,
                WordLength = wordLengthValue,
            };
        }

        private double combinedDifficultyValue(FingerControl fingerControl, KeyTravel keyTravel, Retrigger retrigger, RowSwitch rowSwitch, Speed speed, WordLength wordLength)
        {
            List<double> peaks = combinePeaks(
                fingerControl.GetCurrentStrainPeaks().ToList(),
                keyTravel.GetCurrentStrainPeaks().ToList(),
                retrigger.GetCurrentStrainPeaks().ToList(),
                rowSwitch.GetCurrentStrainPeaks().ToList(),
                speed.GetCurrentStrainPeaks().ToList(),
                wordLength.GetCurrentStrainPeaks().ToList()
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

        private List<double> combinePeaks(List<double> fingerControlPeaks, List<double> keyTravelPeaks, List<double> retriggerPeaks, List<double> rowSwitchPeaks, List<double> speedPeaks, List<double>
                                              wordLengthPeaks)
        {
            int max = wordLengthPeaks.Count;
            var combined = new List<double>(max);

            for (int i = 0; i < max; i++)
            {
                double fingerControlPeak = i < fingerControlPeaks.Count ? fingerControlPeaks[i] : 0;
                double keyTravelPeak = i < keyTravelPeaks.Count ? keyTravelPeaks[i] : 0;
                double retriggerPeak = i < retriggerPeaks.Count ? retriggerPeaks[i] : 0;
                double rowSwitchPeak = i < rowSwitchPeaks.Count ? rowSwitchPeaks[i] : 0;
                double speedPeak = i < speedPeaks.Count ? speedPeaks[i] : 0;
                double wordLengthPeak = i < wordLengthPeaks.Count ? wordLengthPeaks[i] : 0;

                combined.Add(fingerControlPeak * finger_control_skill_multiplier
                             + keyTravelPeak * key_travel_skill_multiplier
                             + retriggerPeak * retrigger_skill_multiplier
                             + rowSwitchPeak * row_switch_skill_multiplier
                             + speedPeak * speed_skill_multiplier
                             + wordLengthPeak * word_length_skill_multiplier);
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

            return objects;
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
        {
            return new Skill[]
            {
                new FingerControl(mods),
                new KeyTravel(mods),
                new Retrigger(mods),
                new RowSwitch(mods),
                new Speed(mods),
                new WordLength(mods),
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
