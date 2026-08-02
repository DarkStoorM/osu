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
        private readonly OsuSpriteText keyMissedCountText;
        private readonly OsuSpriteText keyCountText;
        private readonly OsuSpriteText keyCountSeparatorText;
        private readonly OsuSpriteText keyText;
        private readonly OsuSpriteText unstableRateText;
        private readonly Box cardBox;

        public KeyboardKeyCard(string key, KeyCardData keyCardData)
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
                            Text = setUnstableRateText(keyCardData.UnstableRate),
                            Font = OsuFont.Torus.With(size: 20),
                        },

                        keyText = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Text = key,
                            Font = OsuFont.Inter.With(size: 50, weight: FontWeight.Bold),
                        },

                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Padding = new MarginPadding { Top = 5 },
                            Children = new[]
                            {
                                keyCountText = new OsuSpriteText
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.Centre,
                                    Text = keyCardData.HitEventsCount.ToString(),
                                    Font = OsuFont.Numeric.With(size: 14, weight: FontWeight.Bold),
                                },
                                keyCountSeparatorText = new OsuSpriteText
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.Centre,
                                    Text = "/",
                                    Font = OsuFont.Numeric.With(size: 14, weight: FontWeight.Bold),
                                },
                                keyMissedCountText = new OsuSpriteText
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.Centre,
                                    Text = keyCardData.MissEventsCount.ToString(),
                                    Font = OsuFont.Numeric.With(size: 14, weight: FontWeight.Bold),
                                },
                            }
                        }
                    }
                }
            };

            updateCardColours(keyCardData);
        }

        private static string setUnstableRateText(double unstableRate) => unstableRate == 0 ? "N/A" : $"UR: {unstableRate:F0}";

        public void UpdateKeyCard(KeyCardData keyCardData)
        {
            unstableRateText.Text = setUnstableRateText(keyCardData.UnstableRate);
            keyCountText.Text = keyCardData.HitEventsCount.ToString();
            keyMissedCountText.Text = keyCardData.MissEventsCount.ToString();

            updateCardColours(keyCardData);
        }

        private void updateCardColours(KeyCardData keyCardData)
        {
            BorderColour = keyCardData.BorderColour;

            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = keyCardData.GlowColour,
                Radius = 4,
            };

            cardBox.Colour = keyCardData.BackgroundColour;
            unstableRateText.Colour = keyCardData.TextColour;
            keyText.Colour = keyCardData.TextColour;
            keyCountText.Colour = keyCardData.TextColour;
            keyMissedCountText.Colour = keyCardData.TextColour;
            keyCountSeparatorText.Colour = keyCardData.TextColour;
        }
    }
}
