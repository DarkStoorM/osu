// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Rulesets.Typing.Screens.Ranking.Statistics
{
    public partial class KeyboardKeyCard : Container
    {
        public KeyboardKeyCard(string key, int pressCount, double? unstableRate, Colour4 colour)
        {
            var cardColour = unstableRate == null ? Colour4.Cyan : colour;

            Width = 75;
            Height = 90;
            Masking = true;
            CornerRadius = 8;
            BorderThickness = 1.5f;
            BorderColour = cardColour.Opacity(0.5f);
            Margin = new MarginPadding(5);

            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = cardColour.Opacity(0.12f),
                Radius = 4,
            };

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = cardColour.Opacity(0.2f),
                },

                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(10),

                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Padding = new MarginPadding { Top = 5 },
                            Text = unstableRate == null ? "N/A" : $"UR: {unstableRate:F0}",
                            Font = OsuFont.Torus.With(size: 20),
                        },

                        new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Text = key,
                            Font = OsuFont.Inter.With(size: 50, weight: FontWeight.Bold),
                        },

                        new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Text = pressCount.ToString(),
                            Font = OsuFont.Numeric.With(size: 14, weight: FontWeight.Bold),
                        },
                    }
                }
            };
        }
    }
}
