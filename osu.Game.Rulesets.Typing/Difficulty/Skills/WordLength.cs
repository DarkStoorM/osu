// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    /// <summary>
    /// Skill only applying a penalty based on the word length of the current object, without any awareness of other factors,
    /// e.g. word complexity, etc.
    /// </summary>
    public class WordLength : StrainSkill
    {
        private double skillMultiplier => 0.2;
        private double strainDecayBase => 0.7;
        private double currentStrain;

        public WordLength(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            var currentObject = (TypingHitObject)current.BaseObject;

            double length = currentObject.WordLength;
            double index = currentObject.IndexInWord;

            if (length <= 0)
                return 0;

            double wordProgress = index / length;

            // Longer words should be disproportionately harder (I think?)
            // Also, technically, this could be capped above some length, but since the longest words exist in 5k/25k dictionaries
            // and are rare uncommon anyway, there is no need to do anything more here.
            // Still, the speed should affect this strain at least in some way, but it might yield similar
            // results to typing fatigue
            double lengthFactor = Math.Pow(length, 1.125);

            currentStrain *= strainDecay(current.DeltaTime);
            currentStrain += lengthFactor * wordProgress * skillMultiplier;

            return currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
