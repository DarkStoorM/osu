// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class Speed : StrainSkill
    {
        private double skillMultiplier => 2;
        private double strainDecayBase => 0.075;
        private double currentStrain;

        public Speed(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            TypingHitObject currentHitObject = (TypingHitObject)current.BaseObject;

            currentStrain *= strainDecay(current.DeltaTime);

            double noteDensityPenalty = 0.02 * Math.Pow(1000.0 / current.DeltaTime, 2.25);

            currentStrain += 1.5 + DiffUtils.Logistic(currentHitObject.IndexInWord, 0, 1)
                * skillMultiplier
                * noteDensityPenalty;

            return currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
