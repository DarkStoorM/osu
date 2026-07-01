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
    /// <summary>
    /// Skill measuring the key repetition and applying more strain based on how fast the same key was pressed.
    /// </summary>
    public class Retrigger : StrainSkill
    {
        public Retrigger(Mod[] mods)
            : base(mods) { }

        private double skillMultiplier => 8;
        private double strainDecayBase => 0.5;
        private double currentStrain;

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            TypingDifficultyHitObject currentObject = (TypingDifficultyHitObject)current;
            TypingHitObject currentHitObject = (TypingHitObject)current.BaseObject;

            currentStrain *= strainDecay(current.DeltaTime);

            if (!currentObject.IsKeyRepeated)
                return currentStrain;

            // If the key was repeated, but it was the first letter in the word, e.g. `I IS` or `HI I IT`, we should still
            // count it as a retrigger, but with reduced strain. The goal was to count only the repeated letters inside
            // a word, but since `I` and `A` exist in dictionaries as words, we can still use them, taking the increased
            // spacing between them into account
            double spacingMultiplier = currentHitObject.IndexInWord == 1 ? 0.75 : 1;
            double fingerDifficultyLogistic = 1 + DifficultyCalculationUtils.Logistic((double)currentObject.PhysicalKey.Finger, 5, 1);

            currentStrain += fingerDifficultyLogistic * spacingMultiplier * skillMultiplier;

            return currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
