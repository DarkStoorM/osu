// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty.Components
{
    public partial class InfoCell : Container
    {
        public InfoCell(int fontSize, string text, Colour4 colour)
        {
            RelativeSizeAxes = Axes.Both;

            Child = new OsuSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = OsuFont.Torus.With(size: fontSize),
                Text = text,
                Colour = colour
            };
        }
    }
}
