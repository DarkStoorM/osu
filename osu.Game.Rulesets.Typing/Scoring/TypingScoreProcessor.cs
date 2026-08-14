// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
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
        private const double bonus_space_score_fraction = 0.15;
        private const double bonus_space_score_budget = MAX_SCORE * bonus_space_score_fraction;

        /// <summary>
        /// Will grant extra score above this value of Accuracy.
        /// </summary>
        private const int min_overall_difficulty_for_bonus = 5;

        private const float max_overall_difficulty_bonus = 0.10f;

        private double perSpaceBonus;
        private double overallDifficultyMultiplier;

        public TypingScoreProcessor()
            : base(new TypingRuleset())
        {
        }

        // Same as in osu!taiko
        protected override double ComputeTotalScore(double comboProgress, double accuracyProgress, double bonusPortion)
        {
            double totalScore = 250000 * comboProgress
                                + 750000 * DiffUtils.Pow(Accuracy.Value, 3.6) * accuracyProgress;

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
        /// Returns the multiplier to be applied to the total score, which at most can be <see cref="max_overall_difficulty_bonus"/>,
        /// and is calculated based on the clamped Overall Difficulty of the beatmap, so the bonus is awarded only above some
        /// set value.
        /// </summary>
        private double calculateOverallDifficultyMultiplier(float? overallDifficulty)
        {
            if (overallDifficulty == null)
                return 1;

            float odClamped = Math.Clamp((float)overallDifficulty, min_overall_difficulty_for_bonus, 10) - min_overall_difficulty_for_bonus;

            return 1.0 + odClamped / min_overall_difficulty_for_bonus * max_overall_difficulty_bonus;
        }

        protected override double GetBonusScoreChange(JudgementResult result) => perSpaceBonus;
    }
}
