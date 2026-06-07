// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Difficulty.Skills;
using osu.Game.Rulesets.Typing.Mods;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public class TypingDifficultyCalculator : DifficultyCalculator
    {
        private const double difficulty_multiplier = 1;

        public override int Version => 20260607;

        public TypingDifficultyCalculator(IRulesetInfo ruleset, IWorkingBeatmap beatmap)
            : base(ruleset, beatmap)
        {
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

            var strain = skills.OfType<Speed>().Single();
            var speed = skills.OfType<Strain>().Single();
            var retrigger = skills.OfType<Retrigger>().Single();

            double strainValue = strain.DifficultyValue();
            double speedValue = speed.DifficultyValue();
            double retriggerValue = retrigger.DifficultyValue();

            double starRating = (strainValue + speedValue + retriggerValue) * difficulty_multiplier;

            return new TypingDifficultyAttributes
            {
                StarRating = starRating,
                Mods = mods,
                MaxCombo = beatmap.GetMaxCombo(),
                Strain = strainValue,
                Speed = speedValue,
                Retrigger = retriggerValue
            };
        }

        protected override IEnumerable<DifficultyHitObject> CreateDifficultyHitObjects(IBeatmap beatmap, double clockRate)
        {
            var objects = new List<DifficultyHitObject>();

            for (int i = 1; i < beatmap.HitObjects.Count; i++)
            {
                objects.Add(
                    new TypingDifficultyHitObject(
                        beatmap.HitObjects[i],
                        beatmap.HitObjects[i - 1],
                        clockRate,
                        objects,
                        objects.Count
                    )
                );
            }

            return objects;
        }

        protected override Skill[] CreateSkills(IBeatmap beatmap, Mod[] mods, double clockRate)
        {
            return new Skill[]
            {
                new Speed(mods),
                new Strain(mods),
                new Retrigger(mods)
            };
        }
    }
}
