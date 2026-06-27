// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
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
            ColorGradient ??= new ColorGradient(
                new ColorStop(0, new Colour4(0f, 0.85f, 0.2f, 1f)),
                new ColorStop(150, new Colour4(0.6f, 1f, 0.2f, 1f)),
                new ColorStop(225, new Colour4(1f, 0.8f, 0f, 1f)),
                new ColorStop(300, new Colour4(1f, 0.2f, 0.2f, 1f))
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
                double? unstableRate = keyHitEvents.CalculateKeyUnstableRate(key.Character)?.Result * RNG.NextDouble(0.1, 0.7);

                keyCards.Add(new KeyboardKeyCard(key.Character.ToString(),
                        keyHitEvents.Count,
                        unstableRate,
                        ColorGradient.Evaluate(unstableRate ?? 0)
                    )
                );
            }

            return keyCards.ToArray();
        }
    }
}
