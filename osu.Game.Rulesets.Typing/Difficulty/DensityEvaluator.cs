// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public static class DensityEvaluator
    {
        public static double EvaluateDifficultyOf(DifficultyHitObject current)
        {
            // Stacked objects should probably contribute nothing(?)
            if (current.DeltaTime == 0)
                return 0;

            double difficulty = 0.08 * (1000 / current.DeltaTime);

            return Math.Clamp(difficulty, 0, 1.09);
        }
    }
}
