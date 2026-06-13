// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty.Components
{
    public partial class StatisticsBar : GridContainer
    {
        public StatisticsBar(int fontSize, Colour4[] colours, params string[] cells)
        {
            RelativeSizeAxes = Axes.Both;

            ColumnDimensions = cells
                               .Select(_ => new Dimension())
                               .ToArray();
            int index = 0;
            Content = new[]
            {
                cells.Select(text =>
                     {
                         var colour = colours.Length > 0 ? colours[index++] : Colour4.White;

                         return new InfoCell(fontSize, text, colour);
                     })
                     .ToArray()
            };
        }
    }
}
