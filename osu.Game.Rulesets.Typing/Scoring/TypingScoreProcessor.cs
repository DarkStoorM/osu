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

        /// <summary>
        /// Overall Difficulty (or Accuracy) where the score multiplier is 1. Below this value, multiplier is negative, positive above it.
        /// </summary>
        private const float overall_difficulty_mid_point = 5;

        private const float overall_difficulty_multiplier = 0.25f;

        private double perSpaceBonus;
        private double overallDifficultyMultiplier;

        public TypingScoreProcessor()
            : base(new TypingRuleset())
        {
        }

        protected override double ComputeTotalScore(double comboProgress, double accuracyProgress, double bonusPortion)
        {
            double totalScore = 200000 * comboProgress
                                + 800000 * DiffUtils.Pow(Accuracy.Value, 4) * accuracyProgress;

            // The Overall Difficulty multiplier is applied to incentivise more accurate plays
            return totalScore * overallDifficultyMultiplier + bonusPortion;
        }

        public override void ApplyBeatmap(IBeatmap beatmap)
        {
            base.ApplyBeatmap(beatmap);

            int spaceCount = beatmap.HitObjects
                                    .OfType<SpaceHitObject>()
                                    .Count();

            perSpaceBonus = spaceCount > 0 ? bonus_space_score_budget / spaceCount : 0;
            overallDifficultyMultiplier = calculateOverallDifficultyMultiplier(beatmap.Difficulty.OverallDifficulty);
        }

        public override int GetBaseScoreForResult(HitResult result)
        {
            // Similar to osu!Mania, but it should be expected to hit the Great more than Perfect, so, let
            // the Perfects take a bigger portion of the max score. This is somewhat related to the
            // Overall Difficulty bonus, because it's more rewarding to be more accurate than average Greats
            switch (result)
            {
                case HitResult.Perfect:
                    return 325;
            }

            return base.GetBaseScoreForResult(result);
        }

        /// <summary>
        /// Returns the multiplier to be applied to the total score, which at most can be <see cref="overall_difficulty_multiplier"/>.
        /// <para/>This creates a range of score multipliers from <c>-multiplier to +multiplier</c>, resulting in an example
        /// total score of <c>750000 up to 1250000</c>.
        /// <para/>This is purely for balance purposes, because zero OD SS play could grant 1000000 score if played perfectly,
        /// making it unfair against OD 10 plays, which could barely scratch 1000000 with accuracy drops.
        /// <para/>Bonus value for OD was also increased for this reason.
        /// </summary>
        private double calculateOverallDifficultyMultiplier(float? overallDifficulty)
        {
            if (overallDifficulty == null)
                return 1;

            return 1.0 + ((float)overallDifficulty - overall_difficulty_mid_point) / overall_difficulty_mid_point * overall_difficulty_multiplier;
        }

        protected override double GetBonusScoreChange(JudgementResult result) => perSpaceBonus;
    }
}
