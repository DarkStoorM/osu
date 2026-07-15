// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Typing.Scoring
{
    /// <summary>
    /// Typing HitWindows based on osu!mania
    /// </summary>
    public class TypingHitWindows : HitWindows
    {
        public static readonly DifficultyRange PERFECT_WINDOW_RANGE = new DifficultyRange(55, 34, 24);
        public static readonly DifficultyRange GREAT_WINDOW_RANGE = new DifficultyRange(95, 60, 38);
        public static readonly DifficultyRange GOOD_WINDOW_RANGE = new DifficultyRange(130, 90, 53);
        public static readonly DifficultyRange OK_WINDOW_RANGE = new DifficultyRange(160, 120, 69);
        public static readonly DifficultyRange MEH_WINDOW_RANGE = new DifficultyRange(185, 145, 88);
        public static readonly DifficultyRange MISS_WINDOW_RANGE = new DifficultyRange(220, 175, 109);

        private double perfect;
        private double great;
        private double good;
        private double ok;
        private double meh;
        private double miss;

        public override bool IsHitResultAllowed(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                case HitResult.Great:
                case HitResult.Good:
                case HitResult.Ok:
                case HitResult.Meh:
                case HitResult.Miss:
                    return true;
            }

            return false;
        }

        public override void SetDifficulty(double difficulty)
        {
            perfect = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, PERFECT_WINDOW_RANGE)) - 0.5;
            great = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, GREAT_WINDOW_RANGE)) - 0.5;
            good = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, GOOD_WINDOW_RANGE)) - 0.5;
            ok = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, OK_WINDOW_RANGE)) - 0.5;
            meh = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, MEH_WINDOW_RANGE)) - 0.5;
            miss = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, MISS_WINDOW_RANGE)) - 0.5;
        }

        public override double WindowFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return perfect;

                case HitResult.Great:
                    return great;

                case HitResult.Good:
                    return good;

                case HitResult.Ok:
                    return ok;

                case HitResult.Meh:
                    return meh;

                case HitResult.Miss:
                    return miss;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
    }
}
