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
        private double skillMultiplier => 0.5;
        private double strainDecayBase => 0.35;
        private double currentStrain;

        public Speed(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            TypingHitObject currentHitObject = (TypingHitObject)current.BaseObject;

            // Probably not much to do here anyway
            currentStrain *= strainDecay(current.DeltaTime);
            currentStrain += 1.5 + DifficultyCalculationUtils.Logistic(currentHitObject.IndexInWord, 3, 1) * skillMultiplier;

            return currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
