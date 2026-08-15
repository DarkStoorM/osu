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
                    return 1.5;

                case HitResult.Great:
                    return 3;

                // Due to the Perfect window being almost twice as tight as Great on higher ODs, and stable typing
                // on higher speeds being quite hard, the health increase for Perfects should be higher.
                // This is tuned specifically for later gameplay above 120~WPM
                case HitResult.Perfect:
                    return 3.5;

                default:
                    return 0;
            }
        }
    }
}
