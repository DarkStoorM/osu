// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Typing.Screens.Ranking.Statistics
{
    // Note: the key card had a hard-coded size for now until a proper solution is in place for automatic scaling, which I probably
    // won't implement, because I hastily added this to have something on the result screen
    /// <summary>
    /// A "Card" representing a keyboard key, which shows the unstable rate in top row, a count of successful hits + missed in bottom row.
    /// </summary>
    public partial class KeyboardKeyCard : Container
    {
        private const float card_size = 55;
        private const float additional_information_box_height = 16;

        private readonly OsuSpriteText keyMissedCountText;
        private readonly OsuSpriteText keyCountText;
        private readonly OsuSpriteText keyText;
        private readonly OsuSpriteText unstableRateText;

        private readonly Box cardBox;
        private readonly Box unstableRateBox;
        private readonly Box hitCountBox;

        private readonly Container cardContainer;
        private readonly Container unstableRateContainer;
        private readonly Container hitCountContainer;

        private readonly Color4 unstableRateTextColour = Color4Extensions.FromHex("d9d450");
        private readonly Color4 keysHitCountTextColour = Color4Extensions.FromHex("7fcc33");
        private readonly Color4 hitCountSeparatorTextColour = Color4Extensions.FromHex("d9d450");
        private readonly Color4 keysMissedCountTextColour = Color4Extensions.FromHex("eb4747");

        public KeyboardKeyCard(string key, KeyCardData keyCardData)
        {
            Width = card_size;
            Height = 35 + additional_information_box_height * 2;
            Margin = new MarginPadding(5);

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(0),

                    Children = new Drawable[]
                    {
                        unstableRateContainer = new Container
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Width = card_size - 8,
                            Height = additional_information_box_height,
                            Masking = true,
                            CornerRadius = 4,
                            BorderThickness = 1.5f,
                            Margin = new MarginPadding { Bottom = -8 },

                            Children = new Drawable[]
                            {
                                unstableRateBox = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                unstableRateText = new OsuSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = setUnstableRateText(keyCardData.UnstableRate),
                                    Font = OsuFont.Torus.With(size: 13),
                                    Colour = unstableRateTextColour
                                }
                            }
                        },

                        cardContainer = new Container
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Width = card_size,
                            Height = card_size,
                            Masking = true,
                            CornerRadius = 8,
                            BorderThickness = 1.5f,
                            Depth = 1,

                            Children = new Drawable[]
                            {
                                cardBox = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                keyText = new OsuSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = key,
                                    Font = OsuFont.Inter.With(size: 40, weight: FontWeight.Bold),
                                }
                            }
                        },

                        hitCountContainer = new Container
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Width = card_size - 8,
                            Height = additional_information_box_height,
                            Masking = true,
                            CornerRadius = 4,
                            BorderThickness = 1.5f,
                            Margin = new MarginPadding { Top = -8 },

                            Children = new Drawable[]
                            {
                                hitCountBox = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Padding = new MarginPadding(5),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,

                                    Children = new Drawable[]
                                    {
                                        keyCountText = additionalInformationText(keyCardData.HitEventsCount.ToString(), keysHitCountTextColour),

                                        // The separator never changes, so no need to store it
                                        additionalInformationText("/", hitCountSeparatorTextColour),

                                        keyMissedCountText = additionalInformationText(keyCardData.MissEventsCount.ToString(), keysMissedCountTextColour)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            updateCardColours(keyCardData);
        }

        private static OsuSpriteText additionalInformationText(string content, Colour4 colour)
        {
            return new OsuSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = content,
                Font = OsuFont.Torus.With(size: 13, weight: FontWeight.Bold),
                Colour = colour
            };
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

            cardContainer.EdgeEffect = setEdgeEffect(keyCardData);
            unstableRateContainer.EdgeEffect = setEdgeEffect(keyCardData);
            hitCountContainer.EdgeEffect = setEdgeEffect(keyCardData);

            cardBox.Colour = keyCardData.BackgroundColour;
            unstableRateBox.Colour = keyCardData.AdditionalInformationBackgroundColour;
            hitCountBox.Colour = keyCardData.AdditionalInformationBackgroundColour;
            keyText.Colour = keyCardData.TextColour;
        }

        private static EdgeEffectParameters setEdgeEffect(KeyCardData keyCardData)
        {
            return new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = keyCardData.GlowColour,
                Radius = 4,
            };
        }
    }
}
