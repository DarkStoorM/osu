// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Difficulty.Skills;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Utils;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public class TypingDifficultyCalculator : DifficultyCalculator
    {
        private const double difficulty_multiplier = 0.005;
        private const double speed_skill_multiplier = 1.5 * difficulty_multiplier;
        private const double key_travel_skill_multiplier = 0.8 * difficulty_multiplier;
        private const double row_switch_skill_multiplier = 0.725 * difficulty_multiplier;
        private const double word_length_skill_multiplier = 1.5 * difficulty_multiplier;
        private const double typing_fatigue_skill_multiplier = 0.875 * difficulty_multiplier;
        private const double retrigger_skill_multiplier = 0.375 * difficulty_multiplier;

        public override int Version => 20260607;

        public TypingDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
            : base(ruleset, beatmap) { }

        protected override Mod[] DifficultyAdjustmentMods => new Mod[]
        {
            new TypingModDoubleTime(),
            new TypingModHalfTime(),
        };

        protected override DifficultyAttributes CreateDifficultyAttributes(IBeatmap beatmap, Mod[] mods, Skill[] skills)
        {
            // Difficulty calculation will not be available if the word generation mods are not enabled,
            // because this ruleset is meant to be played only with those mods due to some skills
            // relying on letters being a part of a word, which in this context form a pattern
            if (!mods.Any(x => x is TypingModWords) || beatmap.HitObjects.Count == 0)
                return new TypingDifficultyAttributes { Mods = mods };

            var keyTravel = skills.OfType<KeyTravel>().Single();
            var retrigger = skills.OfType<Retrigger>().Single();
            var rowSwitch = skills.OfType<RowSwitch>().Single();
            var speed = skills.OfType<Speed>().Single();
            var typingFatigue = skills.OfType<TypingFatigue>().Single();
            var wordLength = skills.OfType<WordLength>().Single();

            double keyTravelValue = keyTravel.DifficultyValue();
            double retriggerValue = retrigger.DifficultyValue();
            double rowSwitchValue = rowSwitch.DifficultyValue();
            double speedValue = speed.DifficultyValue();
            double typingFatigueValue = typingFatigue.DifficultyValue();
            double wordLengthValue = wordLength.DifficultyValue();
            double combinedRating = combinedDifficultyValue(keyTravel, retrigger, rowSwitch, speed, typingFatigue, wordLength);
            double starRating = rescale(combinedRating * 1.4);

            // A hacky way perhaps, but I don't have anything else to not make the difficulty explode on short beatmaps
            // Also note: at 900 objects, the scale reaches 92%, which roughly evaluates to the following:
            // - 120 seconds of playing 140 bpm with halved beat length (or 280 on full beat length)
            // - 90 seconds of playing 180 bpm
            const double optimal_maximum_object_count = 1200;
            double objectCountRatio = beatmap.HitObjects.Count / optimal_maximum_object_count;
            double lengthScale = DiffUtils.Logistic(4 * objectCountRatio, 0.5, 1);

            starRating *= lengthScale;

            return new TypingDifficultyAttributes
            {
                StarRating = starRating,
                Mods = mods,
                MaxCombo = beatmap.GetMaxCombo(),
                KeyTravel = keyTravelValue,
                Retrigger = retriggerValue,
                RowSwitch = rowSwitchValue,
                Speed = speedValue,
                TypingFatigue = typingFatigueValue,
                WordLength = wordLengthValue,

                KeyTravelSkill = keyTravel,
                RetriggerSkill = retrigger,
                RowSwitchSkill = rowSwitch,
                SpeedSkill = speed,
                TypingFatigueSkill = typingFatigue,
                WordLengthSkill = wordLength,
            };
        }

        private double combinedDifficultyValue(KeyTravel keyTravel,
                                               Retrigger retrigger,
                                               RowSwitch rowSwitch,
                                               Speed speed,
                                               TypingFatigue typingFatigue,
                                               WordLength wordLength)
        {
            List<double> peaks = combinePeaks(
                keyTravel.GetCurrentStrainPeaks().ToList(),
                retrigger.GetCurrentStrainPeaks().ToList(),
                rowSwitch.GetCurrentStrainPeaks().ToList(),
                speed.GetCurrentStrainPeaks().ToList(),
                typingFatigue.GetCurrentStrainPeaks().ToList(),
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

        private List<double> combinePeaks(IReadOnlyList<double> keyTravelPeaks,
                                          IReadOnlyList<double> retriggerPeaks,
                                          IReadOnlyList<double> rowSwitchPeaks,
                                          IReadOnlyList<double> speedPeaks,
                                          IReadOnlyList<double> typingFatiguePeaks,
                                          IReadOnlyList<double> wordLengthPeaks)
        {
            int max = wordLengthPeaks.Count;
            var combined = new List<double>(max);

            for (int i = 0; i < max; i++)
            {
                double keyTravelPeak = i < keyTravelPeaks.Count ? keyTravelPeaks[i] : 0;
                double retriggerPeak = i < retriggerPeaks.Count ? retriggerPeaks[i] : 0;
                double rowSwitchPeak = i < rowSwitchPeaks.Count ? rowSwitchPeaks[i] : 0;
                double speedPeak = i < speedPeaks.Count ? speedPeaks[i] : 0;
                double typingFatiguePeak = i < typingFatiguePeaks.Count ? typingFatiguePeaks[i] : 0;
                double wordLengthPeak = i < wordLengthPeaks.Count ? wordLengthPeaks[i] : 0;

                combined.Add(keyTravelPeak * key_travel_skill_multiplier
                             + retriggerPeak * retrigger_skill_multiplier
                             + rowSwitchPeak * row_switch_skill_multiplier
                             + speedPeak * speed_skill_multiplier
                             + typingFatiguePeak * typing_fatigue_skill_multiplier
                             + wordLengthPeak * word_length_skill_multiplier);
            }

            return combined;
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, Mod[] mods)
        {
            var objects = new List<DifficultyHitObject>();
            double clockRate = ModUtils.CalculateRateWithMods(mods);

            for (int i = 2; i < beatmap.HitObjects.Count; i++)
            {
                objects.Add(
                    new TypingDifficultyHitObject(
                        beatmap.HitObjects[i],
                        beatmap.HitObjects[i - 1],
                        clockRate,
                        objects,
                        objects.Count,
                        beatmap.HitObjects.ElementAtOrDefault(i + 1)
                    )
                );
            }

            return objects;
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods)
        {
            return new Skill[]
            {
                new KeyTravel(mods),
                new Retrigger(mods),
                new RowSwitch(mods),
                new Speed(mods),
                new TypingFatigue(mods),
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
