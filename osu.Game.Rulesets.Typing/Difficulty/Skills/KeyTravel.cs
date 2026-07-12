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
        private double skillMultiplier => 0.4;
        private double strainDecayBase => 0.2;
        private double currentStrain;

        public KeyTravel(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            TypingDifficultyHitObject currentObject = (TypingDifficultyHitObject)current;
            TypingDifficultyHitObject previousObject = (TypingDifficultyHitObject)current.Previous();
            TypingHitObject currentHitObject = (TypingHitObject)current.BaseObject;

            currentStrain *= strainDecay(current.DeltaTime);

            if (previousObject == null || current.Index == 0 || !currentObject.IsOnSameHand)
                return 0;

            double spacingMultiplier = currentHitObject.IndexInWord == 1 ? 0.75 : 1;
            double travelDifficulty = 1;

            // Horizontal movement is considered easier than vertical, so we only apply bigger strain above one row delta
            if (!currentObject.IsOnSameRow)
                travelDifficulty = 1 + DiffUtils.Logistic(currentObject.RowDelta, 2, 1);

            double previousFinger = (double)previousObject.PhysicalKey.Finger;
            double currentFinger = (double)currentObject.PhysicalKey.Finger;

            // The difficulty increases if we go in the opposite rolling direction
            if (currentFinger > previousFinger)
            {
                double fingerDelta = Math.Abs(previousFinger - currentFinger);
                double fingerDifficulty = 1 + DiffUtils.Logistic(1.25 * currentFinger, 3, 1);
                travelDifficulty *= fingerDifficulty * (1 + DiffUtils.Logistic(fingerDelta, 3, 1));
            }

            // Introduce an extra, small penalty for movement within the same hand, the longer the word is, which
            // is maximised for words longer than 5 letters
            double lengthMultiplier = 1 + 0.25 * DiffUtils.Logistic(currentHitObject.IndexInWord, 3, 1);

            currentStrain += skillMultiplier
                             * currentObject.DistanceFromPreviousKey
                             * travelDifficulty
                             * spacingMultiplier
                             * lengthMultiplier;

            return currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
