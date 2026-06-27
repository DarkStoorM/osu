// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Typing.Screens.Ranking.Statistics
{
    public partial class KeyboardKeyCard : Container
    {
        public KeyboardKeyCard(string key, int pressCount, double? unstableRate)
        {
            Anchor = Anchor.TopLeft;
            Origin = Anchor.TopLeft;
            Size = new Vector2(70, 80);

            Masking = true;

            CornerRadius = 6;
            BorderThickness = 1;
            BorderColour = Color4.Cyan;
            Margin = new MarginPadding(5);
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = new Colour4(0, 255, 255, 100),
                Radius = 4,
            };

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(10, 10, 14, 255)
                },

                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(6),
                    Spacing = new Vector2(2),
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Text = unstableRate == null ? "N/A" : $"UR: {unstableRate:F0}",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        },

                        new OsuSpriteText
                        {
                            Text = key,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = new FontUsage(OsuFont.Torus.ToString(), size: 50, weight: "Bold"),
                            Padding = new MarginPadding { Bottom = -10 }
                        },

                        new OsuSpriteText
                        {
                            Text = $"{pressCount.ToString()}",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = new FontUsage(OsuFont.Numeric.ToString(), size: 20),
                        }
                    }
                }
            };
        }
    }
}
