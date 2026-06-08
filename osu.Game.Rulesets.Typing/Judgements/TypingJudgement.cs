// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Typing.Judgements
{
    public class TypingJudgement : Judgement
    {
        protected override double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Miss:
                    return -0.5;

                case HitResult.Meh:
                    return -0.25;

                case HitResult.Ok:
                    return 0.5;

                case HitResult.Good:
                    return 1.0;

                case HitResult.Great:
                    return 2.5;

                case HitResult.Perfect:
                    return 3.0;

                default:
                    return 0;
            }
        }
    }
}
