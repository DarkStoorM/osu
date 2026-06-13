// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty.Components
{
    public partial class GraphContainer : Container
    {
        public GraphContainer()
        {
            RelativeSizeAxes = Axes.Both;

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0.1f,
            };
        }
    }
}
