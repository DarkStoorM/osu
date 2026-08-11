// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Typing.Judgements
{
    public class SpaceJudgement : Judgement
    {
        public override HitResult MaxResult => HitResult.LargeBonus;

        protected override double HealthIncreaseFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.LargeBonus:
                    return 0.5;

                default:
                    return 0;
            }
        }
    }
}
