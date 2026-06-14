// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class RowSwitch : StrainSkill
    {
        private double skillMultiplier => 0.45;
        private double strainDecayBase => 0.4;
        private double currentStrain;

        public RowSwitch(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            TypingDifficultyHitObject currentObject = (TypingDifficultyHitObject)current;
            TypingDifficultyHitObject previousObject = (TypingDifficultyHitObject)current.Previous(0);
            TypingHitObject hitObject = (TypingHitObject)current.BaseObject;

            if (previousObject == null)
                return 0;

            double baseRowDifficulty = 1;

            // Single letter and the first letter in the word introduce a small recovery time, reducing the strain
            if (hitObject.WordLength == 1 || hitObject.IndexInWord == 1)
                baseRowDifficulty = 0.50;

            currentStrain *= strainDecay(current.DeltaTime);
            currentStrain += Math.Pow(1 + Math.Abs(currentObject.RowDelta), baseRowDifficulty) * skillMultiplier;

            // Due to alternating hands also introducing a recovery time, the penalty should be smaller
            currentStrain *= currentObject.IsOnSameHand ? 1.1 : 0.9;

            return currentStrain * rowSwitchDifficulty(currentObject);
        }

        private double rowSwitchDifficulty(TypingDifficultyHitObject currentObject)
        {
            double fingerIndex = (double)currentObject.PhysicalKey.Finger;

            // Switching rows is harder when on the same finger
            double sameFingerPenalty = currentObject.IsOnSameFinger
                ? 2.5
                : 1.5;

            // Switching rows is even harder the more outward finger is used
            double fingerDifficulty = 1 + 1 / (1 + Math.Pow(Math.E, -(fingerIndex - 5)));

            return Math.Pow(fingerDifficulty, sameFingerPenalty);
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
