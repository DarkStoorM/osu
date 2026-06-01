// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Typing.Scoring
{
    /// <summary>
    /// Typing HitWindows based on osu!
    /// </summary>
    public class TypingHitWindows : HitWindows
    {
        // Note on the values and why they are quite larger:
        // There are quite a bit of layers stacked on top of each other, that the cognitive load might be overwhelming.
        // It might seem like it's similar to osu!mania where players are able to play with lots of keys, but we have
        // to take other factors into account:
        // - One finger can map to multiple keys, depending on typing style
        // - Variable finger travel due to the above
        // - Player has to actively parse language
        // - Player has to recognise character, select the finger and map to the correct key
        // - Player has to overcome the habit of unstable chording, forcing the on-beat keypress
        // - Inputs are not equally easy, unlike other rulesets,
        // The rest overlaps with other mods, e.g. hand coordination, but here we have to compensate for the fact,
        // that typing is really, really unstable. Players have to correct themselves, intentionally slowing down from
        // usual typing style.
        public static readonly DifficultyRange GREAT_WINDOW_RANGE = new DifficultyRange(130, 90, 55);
        public static readonly DifficultyRange OK_WINDOW_RANGE = new DifficultyRange(210, 150, 95);
        public static readonly DifficultyRange MEH_WINDOW_RANGE = new DifficultyRange(320, 240, 170);

        public const double MISS_WINDOW = 360;

        private double great;
        private double ok;
        private double meh;

        public override bool IsHitResultAllowed(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                case HitResult.Ok:
                case HitResult.Meh:
                case HitResult.Miss:
                    return true;
            }

            return false;
        }

        public override void SetDifficulty(double difficulty)
        {
            great = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, GREAT_WINDOW_RANGE)) - 0.5;
            ok = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, OK_WINDOW_RANGE)) - 0.5;
            meh = Math.Floor(IBeatmapDifficultyInfo.DifficultyRange(difficulty, MEH_WINDOW_RANGE)) - 0.5;
        }

        public override double WindowFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                    return great;

                case HitResult.Ok:
                    return ok;

                case HitResult.Meh:
                    return meh;

                case HitResult.Miss:
                    return MISS_WINDOW;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }
    }
}
