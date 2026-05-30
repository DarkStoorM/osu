// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Typing.UI
{
    [Cached]
    public partial class TypingPlayfield : ScrollingPlayfield
    {
        private const float lane_height = 150;

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRangeInternal(new Drawable[]
                {
                    new LaneContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Child = new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding
                            {
                                Left = 200,
                                Top = lane_height / 2,
                                Bottom = lane_height / 2
                            },
                            Children = new Drawable[] { HitObjectContainer, }
                        },
                    },
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
            private void load(OsuColour colours)
            {
                InternalChildren = new Drawable[]
                {
                    fill = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Colour = colours.Gray2,
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
                            Colour = Color4.White,
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Height = 0.95f,
                        },
                    };
                }
            }
        }
    }
}
