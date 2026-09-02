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
        public const double BONUS_SPACE_TOTAL_SCORE_FRACTION = 0.25;

        /// <summary>
        /// Value of Overall Difficulty (Accuracy) where the score multiplier is 1.
        /// </summary>
        private const float od_neutral_point = 5f;

        /// <summary>
        /// Maximum score multiplier applied by Overall Difficulty at OD 10.
        /// </summary>
        private const float od_max_multiplier = 0.25f;

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

            perSpaceBonus = spaceCount > 0 ? MAX_SCORE * BONUS_SPACE_TOTAL_SCORE_FRACTION / spaceCount : 0;
            overallDifficultyMultiplier = CalculateOverallDifficultyMultiplier(beatmap.Difficulty.OverallDifficulty);
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
        /// Returns the multiplier to be applied to the total score, which at most can be <see cref="od_max_multiplier"/>, with
        /// an exception to the Extended Limit being applied with the Difficulty Adjustment, where OD can go above the maximum.
        /// <para/>This creates a range of score multipliers from <c>-multiplier to +multiplier</c>, resulting in an example
        /// total score of <c>750000 up to 1250000</c>.
        /// <para/>This is purely for balance purposes, because zero OD SS play could grant 1000000 score if played perfectly,
        /// making it unfair against OD 10 plays, which could barely scratch 1000000 with accuracy drops.
        /// <para/>Bonus value for OD was also increased for this reason.
        /// </summary>
        public static double CalculateOverallDifficultyMultiplier(float? overallDifficulty)
        {
            if (overallDifficulty == null)
                return 1;

            // Round to nearest 0.5
            double od = (int)(overallDifficulty * 2) / 2.0;

            // Warning: currently, this piecewise Lerp will equally distribute the multiplier both ways, because neutral OD is set to 5.
            // The formula will automatically change how the multiplier is distributed if the neutral point gets adjusted.
            // E.g., if 7 is used, adjusting to the right will increment the multiplier more since there are fewer steps to 10.
            // While the maximum is already set, the Extended OD Limit can still bring this up a little.
            // The formula is already in place "just in case" the neutral point is changed
            return od <= od_neutral_point
                ? 1.0
                  - od_max_multiplier
                  + od / od_neutral_point * od_max_multiplier
                : 1.0
                  + (od - od_neutral_point) / (10f - od_neutral_point)
                  * od_max_multiplier;
        }

        protected override double GetBonusScoreChange(JudgementResult result) => perSpaceBonus;
    }
}
