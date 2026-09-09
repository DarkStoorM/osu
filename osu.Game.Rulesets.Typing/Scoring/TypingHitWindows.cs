// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Typing.Scoring
{
    /// <summary>
    /// HitWindows based on osu!mania.
    /// <para/>Speed change affecting the hit windows was also taken from osu!mania.
    /// </summary>
    public class TypingHitWindows : HitWindows
    {
        // Note: It might seem unnecessary to include so many hit windows for something like typing, because all that's really
        // needed is basically a set of windows used by osu!taiko: 300/100/Miss, so, it might be confusing why all these are
        // even displayed to the user. In reality, OK and MEH are not really needed, but since typing can be quite unstable,
        // might just as well include other windows to see the judgement distribution
        public static readonly DifficultyRange PERFECT_WINDOW_RANGE = new DifficultyRange(40, 26, 19);
        public static readonly DifficultyRange GREAT_WINDOW_RANGE = new DifficultyRange(80, 52, 33);
        public static readonly DifficultyRange GOOD_WINDOW_RANGE = new DifficultyRange(115, 82, 48);
        public static readonly DifficultyRange OK_WINDOW_RANGE = new DifficultyRange(145, 112, 64);
        public static readonly DifficultyRange MEH_WINDOW_RANGE = new DifficultyRange(170, 137, 83);
        public static readonly DifficultyRange MISS_WINDOW_RANGE = new DifficultyRange(205, 167, 104);

        private double perfect;
        private double great;
        private double good;
        private double ok;
        private double meh;
        private double miss;

        private double overallDifficulty;
        private double speedMultiplier = 1;

        /// <summary>
        /// Multiplier applied to all HitWindows to retain a somewhat constant window no matter the rate change.
        /// </summary>
        public double SpeedMultiplier
        {
            set
            {
                speedMultiplier = value;
                updateWindows();
            }
        }

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
            overallDifficulty = difficulty;
            updateWindows();
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

        private void updateWindows()
        {
            perfect = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(overallDifficulty, PERFECT_WINDOW_RANGE) * speedMultiplier) + 0.5;
            great = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(overallDifficulty, GREAT_WINDOW_RANGE) * speedMultiplier) + 0.5;
            good = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(overallDifficulty, GOOD_WINDOW_RANGE) * speedMultiplier) + 0.5;
            ok = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(overallDifficulty, OK_WINDOW_RANGE) * speedMultiplier) + 0.5;
            meh = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(overallDifficulty, MEH_WINDOW_RANGE) * speedMultiplier) + 0.5;
            miss = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(overallDifficulty, MISS_WINDOW_RANGE) * speedMultiplier) + 0.5;
        }
    }
}
