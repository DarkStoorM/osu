// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Typing.Judgements
{
    /// <summary>
    /// Similar to Taiko, since there is also Great and Ok in this Ruleset, but the player is more likely to lose accuracy,
    /// so the health loss is a little but more forgiving
    /// </summary>
    public class TypingJudgement : Judgement
    {
        public override HitResult MaxResult => HitResult.Great;

        protected override double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Miss:
                    return -0.75;

                case HitResult.Ok:
                    return 1.5;

                case HitResult.Great:
                    return 3.0;

                default:
                    return 0;
            }
        }
    }
}
