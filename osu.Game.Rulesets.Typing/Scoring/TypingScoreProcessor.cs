// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Scoring
{
    public partial class TypingScoreProcessor : ScoreProcessor
    {
        private const double bonus_space_score_fraction = 0.25;
        private const double bonus_space_score_budget = MAX_SCORE * bonus_space_score_fraction;

        private double perSpaceBonus;

        public TypingScoreProcessor()
            : base(new TypingRuleset())
        {
        }

        // Same as in osu!taiko
        protected override double ComputeTotalScore(double comboProgress, double accuracyProgress, double bonusPortion)
        {
            return 250000 * comboProgress
                   + 750000 * DiffUtils.Pow(Accuracy.Value, 3.6) * accuracyProgress
                   + bonusPortion;
        }

        public override void ApplyBeatmap(IBeatmap beatmap)
        {
            base.ApplyBeatmap(beatmap);

            int spaceCount = beatmap.HitObjects
                                    .OfType<SpaceHitObject>()
                                    .Count();

            perSpaceBonus = spaceCount > 0 ? bonus_space_score_budget / spaceCount : 0;
        }

        protected override double GetBonusScoreChange(JudgementResult result) => perSpaceBonus;
    }
}
