// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty.Components
{
    public partial class StatisticsBar : GridContainer
    {
        public StatisticsBar(int fontSize, params string[] cells)
        {
            RelativeSizeAxes = Axes.Both;

            ColumnDimensions = cells
                               .Select(_ => new Dimension())
                               .ToArray();

            Content = new[]
            {
                cells.Select(Drawable (text) => new InfoCell(fontSize, text))
                     .ToArray()
            };
        }
    }
}
