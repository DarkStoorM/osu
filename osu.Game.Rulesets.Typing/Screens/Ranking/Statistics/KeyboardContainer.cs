// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Typing.Layouts;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osu.Game.Rulesets.Typing.Objects;
using osu.Game.Rulesets.Typing.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Typing.Screens.Ranking.Statistics
{
    public partial class KeyboardContainer : Container
    {
        public static ColorGradient? ColorGradient { get; private set; }

        public KeyboardContainer(ScoreInfo score)
        {
            // A hacky way to create a gradient of color for an arbitrary unstable rate range.
            // The result for <= 1 hits or just zero should show a grayed out color, since
            ColorGradient ??= new ColorGradient(
                new ColorStop(0, new Colour4(0.55f, 0.55f, 0.55f, 1f)),
                new ColorStop(10, new Colour4(0.00f, 0.95f, 0.25f, 1f)),
                new ColorStop(100, new Colour4(0.45f, 1.00f, 0.15f, 1f)),
                new ColorStop(150, new Colour4(1.00f, 0.95f, 0.10f, 1f)),
                new ColorStop(200, new Colour4(1.00f, 0.60f, 0.00f, 1f)),
                new ColorStop(250, new Colour4(1.00f, 0.18f, 0.18f, 1f)),
                new ColorStop(300, new Colour4(0.60f, 0.00f, 0.00f, 1f))
            );

            RelativeSizeAxes = Axes.X;
            Height = 330;

            Child = new KeyDistributionContainer(score)
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
            };
        }

        public partial class KeyDistributionContainer : FillFlowContainer
        {
            public KeyDistributionContainer(ScoreInfo score)
            {
                Direction = FillDirection.Vertical;

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                Children = new Drawable[]
                {
                    new KeyboardRowContainer(score, KeyboardRow.Top),
                    new KeyboardRowContainer(score, KeyboardRow.Home),
                    new KeyboardRowContainer(score, KeyboardRow.Bottom),
                };
            }
        }

        public partial class KeyboardRowContainer : FillFlowContainer
        {
            public KeyboardRowContainer(ScoreInfo score, KeyboardRow row)
            {
                Direction = FillDirection.Horizontal;

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Margin = new MarginPadding { Left = (int)row * 30 };

                Children = getKeyCards(score, row);
            }
        }

        private static KeyboardKeyCard[] getKeyCards(ScoreInfo score, KeyboardRow row)
        {
            KeyboardLayout layout = new QwertyOrtholinearLayout();
            List<KeyboardKeyCard> keyCards = new List<KeyboardKeyCard>();

            var keys = layout.Keys
                             .Select(key => key.Value)
                             .Where(key => key.Row == row)
                             .ToList();

            foreach (PhysicalKey key in keys)
            {
                var keyHitEvents = score.HitEvents
                                        .Where(e => e.HitObject is TypingHitObject hitObject && hitObject.Letter == key.Character)
                                        .ToList();
                double? unstableRate = keyHitEvents.CalculateKeyUnstableRate(key.Character)?.Result ?? 0;

                // Hit events per key should be at least ten to deem the unstable rate valid for this key, so we have to force zero
                // it out if there were not enough keypresses. The reason for this is that the unstable rate will converge at higher
                // amount of hits across the whole gameplay, so a very short beatmap should not yield valuable results
                if (keyHitEvents.Count < 10)
                    unstableRate = null;

                Colour4? colour = ColorGradient?.Evaluate(unstableRate);

                keyCards.Add(new KeyboardKeyCard(key.Character.ToString(),
                        keyHitEvents.Count,
                        unstableRate,
                        colour
                    )
                );
            }

            return keyCards.ToArray();
        }
    }
}
