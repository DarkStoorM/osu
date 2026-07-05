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
        private readonly OsuSpriteText keyCountText;
        private readonly OsuSpriteText keyText;
        private readonly OsuSpriteText unstableRateText;
        private readonly Box cardBox;

        public KeyboardKeyCard(string key, int pressCount, double? unstableRate, Colour4? colour)
        {
            Width = 75;
            Height = 90;
            Masking = true;
            CornerRadius = 8;
            BorderThickness = 1.5f;
            Margin = new MarginPadding(5);

            InternalChildren = new Drawable[]
            {
                cardBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },

                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(10),

                    Children = new Drawable[]
                    {
                        unstableRateText = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Padding = new MarginPadding { Top = 5 },
                            Text = setUnstableRateText(unstableRate),
                            Font = OsuFont.Torus.With(size: 20),
                        },

                        keyText = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Text = key,
                            Font = OsuFont.Inter.With(size: 50, weight: FontWeight.Bold),
                        },

                        keyCountText = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Text = pressCount.ToString(),
                            Font = OsuFont.Numeric.With(size: 14, weight: FontWeight.Bold),
                        },
                    }
                }
            };

            updateCardColours(colour);
        }

        private static string setUnstableRateText(double? unstableRate) => unstableRate == null ? "N/A" : $"UR: {unstableRate:F0}";

        public void UpdateKeyCard(int pressCount, double? unstableRate = null, Colour4? colour = null)
        {
            unstableRateText.Text = setUnstableRateText(unstableRate);
            keyCountText.Text = pressCount.ToString();

            updateCardColours(colour);
        }

        private void updateCardColours(Colour4? colour)
        {
            var fontColour = colour == null ? Colour4.DarkGray.Opacity(0.2f) : Colour4.White;
            var cardColour = colour ?? Colour4.DarkGray.Opacity(0.2f);

            BorderColour = cardColour.Opacity(0.5f);

            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = cardColour.Opacity(0.12f),
                Radius = 4,
            };

            cardBox.Colour = cardColour.Opacity(0.2f);

            unstableRateText.Colour = fontColour;
            keyText.Colour = fontColour;
            keyCountText.Colour = fontColour;
        }
    }
}
