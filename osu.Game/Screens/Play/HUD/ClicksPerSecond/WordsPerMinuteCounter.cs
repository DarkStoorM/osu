// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Screens.Play.HUD.ClicksPerSecond
{
    // Note: this component does not belong in here at all, this is only supposed to be used with
    // TypingRuleset, and it's not a real Words Per Minute counter. It just leverages the
    // ClicksPerSecond component's logic
    public partial class WordsPerMinuteCounter : RollingCounter<int>, ISerialisableDrawable
    {
        [Resolved]
        private WordsPerMinuteController controller { get; set; } = null!;

        public bool UsesFixedAnchor { get; set; }

        public WordsPerMinuteCounter()
        {
            Current.Value = 0;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Colour = colours.BlueLighter;
        }

        protected override void Update()
        {
            base.Update();

            Current.Value = controller.Value;
        }

        protected override IHasText CreateText() => new TextComponent();

        private partial class TextComponent : CompositeDrawable, IHasText
        {
            public LocalisableString Text
            {
                get => text.Text;
                set => text.Text = value;
            }

            private readonly OsuSpriteText text;

            public TextComponent()
            {
                AutoSizeAxes = Axes.Both;

                InternalChild = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Spacing = new Vector2(2),
                    Children = new Drawable[]
                    {
                        text = new OsuSpriteText
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Font = OsuFont.Numeric.With(size: 16, fixedWidth: true)
                        },
                        new FillFlowContainer
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Direction = FillDirection.Vertical,
                            AutoSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    Font = OsuFont.Numeric.With(size: 6, fixedWidth: false),
                                    Text = @"wpm",
                                },
                            }
                        }
                    }
                };
            }
        }
    }
}
