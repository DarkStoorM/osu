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
    public class KeyTravel : StrainSkill
    {
        private double strainDecayBase => 0.3;
        private double currentStrain;

        public KeyTravel(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            TypingDifficultyHitObject currentObject = (TypingDifficultyHitObject)current;
            TypingDifficultyHitObject previousObject = (TypingDifficultyHitObject)current.Previous(0);
            TypingHitObject currentHitObject = (TypingHitObject)current.BaseObject;

            // Distance cross-hand is ignored, because only the movement on the same hand matters
            if (previousObject == null || current.Index == 0 || !currentObject.IsOnSameHand)
                return 0;

            double spacingMultiplier = currentHitObject.IndexInWord == 1 ? 0.75 : 1;
            double travelDifficulty = currentObject.DistanceFromPreviousKey;

            // Horizontal movement is considered easier than vertical, so we apply bigger strain above one row delta
            if (currentObject.IsOnSameRow)
                travelDifficulty *= 2 * DifficultyCalculationUtils.Logistic(currentObject.RowDelta, 0.75);

            double previousFinger = (double)previousObject.PhysicalKey.Finger;
            double currentFinger = (double)currentObject.PhysicalKey.Finger;
            double fingerDelta = 0.5 + Math.Abs(previousFinger - currentFinger) / 3;

            // The difficulty increases if we go in the opposite rolling direction
            if (currentFinger > previousFinger)
                travelDifficulty *= fingerDelta;

            currentStrain *= strainDecay(current.DeltaTime);
            currentStrain += travelDifficulty * spacingMultiplier;

            return currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
