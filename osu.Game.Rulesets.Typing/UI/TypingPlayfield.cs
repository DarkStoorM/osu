// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Typing.UI
{
    [Cached]
    public partial class TypingPlayfield : ScrollingPlayfield
    {
        public static readonly Colour4 LANE_FILL_COLOR = Color4Extensions.FromHex("#1E1E2E");

        private const float judgment_box_width = 70;
        private readonly Colour4 judgmentBoxColour = new Colour4(255, 255, 255, 25);
        private readonly Colour4 judgmentLineColour = Color4Extensions.FromHex("#A6E3A177");

        private const float lane_height = 200;
        private const float lane_left_padding = 200;

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRangeInternal(new Drawable[]
                {
                    new LaneContainer
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding
                            {
                                Left = lane_left_padding,
                                Top = lane_height / 2,
                                Bottom = lane_height / 2
                            },
                            Children = new Drawable[]
                            {
                                HitObjectContainer,
                            }
                        },
                    },
                    // Judgment Line
                    new Box
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding
                        {
                            Left = lane_left_padding,
                        },
                        Height = lane_height,
                        Width = 2,
                        Colour = judgmentLineColour,
                    },
                    // Judgment Box
                    new Box
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding
                        {
                            Left = lane_left_padding - judgment_box_width / 2,
                        },
                        Height = 100,
                        Width = judgment_box_width,
                        Colour = judgmentBoxColour,
                    }
                }
            );
        }

        private partial class LaneContainer : BeatSyncedContainer
        {
            private FillFlowContainer fill = null!;

            private readonly Container content = new Container
            {
                RelativeSizeAxes = Axes.Both,
            };

            protected override Container<Drawable> Content => content;

            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChildren = new Drawable[]
                {
                    fill = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Colour = LANE_FILL_COLOR,
                        Direction = FillDirection.Vertical,
                    },
                    content,
                };

                fill.Add(new Lane
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = lane_height,
                    }
                );
            }

            private partial class Lane : CompositeDrawable
            {
                public Lane()
                {
                    InternalChildren = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Height = 1,
                        },
                    };
                }
            }
        }
    }
}
