// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Play;
using osu.Game.Screens.Play.HUD;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Typing.Skinning
{
    public partial class TypingWpmCounter : RollingCounter<int>, ISerialisableDrawable
    {
        [Resolved]
        private InputCountController inputCountController { get; set; } = null!;

        [Resolved]
        private IGameplayClock gameplayClock { get; set; } = null!;

        [Resolved]
        private IFrameStableClock? frameStableClock { get; set; }

        private IGameplayClock clock => frameStableClock ?? gameplayClock;

        public bool UsesFixedAnchor { get; set; }

        public TypingWpmCounter()
        {
            Current.Value = 0;
            AutoSizeAxes = Axes.Both;
        }

        protected override void Update()
        {
            base.Update();

            double elapsedSeconds = (clock.CurrentTime - gameplayClock.GameplayStartTime) / 1000.0;

            if (elapsedSeconds <= 0)
            {
                Current.Value = 0;
                return;
            }

            int totalInputs = inputCountController.Triggers.Sum(x => x.ActivationCount.Value);

            // Since WPM is calculated from words that are 5 letters long, here, the brief downtime between the words
            // has to be taken into account, which is done by reducing this value to 4. The factor for 5 letters long
            // words is 12 (60 / 5), so the factor for 4 letters long words is 15 (60 / 4).
            int wpm = (int)Math.Round(totalInputs * 15.0 / elapsedSeconds * gameplayClock.GetTrueGameplayRate());

            if (Current.Value != wpm)
                Current.Value = wpm;
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
