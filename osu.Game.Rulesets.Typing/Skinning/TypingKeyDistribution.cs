// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Screens.Ranking.Statistics;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Typing.Skinning
{
    public partial class TypingKeyDistribution : Container, ISerialisableDrawable
    {
        private KeyTimingDistribution keyTimingDistribution = null!;

        public bool UsesFixedAnchor { get; set; }

        [Resolved]
        private ScoreProcessor scoreProcessor { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AutoSizeAxes = Axes.Both;

            keyTimingDistribution = new KeyTimingDistribution(scoreProcessor.HitEvents, scoreProcessor.Mods.Value);

            Child = keyTimingDistribution;
            Child.Alpha = 0.3f;

            scoreProcessor.NewJudgement += updateKeyCard;
            scoreProcessor.JudgementReverted += updateKeyCard;
        }

        private void updateKeyCard(JudgementResult result) => keyTimingDistribution.UpdateKeyCard(result, scoreProcessor.HitEvents);
    }
}
