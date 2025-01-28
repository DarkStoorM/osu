﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Screens.Play.HUD
{
    public partial class HitEventsCounter : RollingCounter<int>, ISerialisableDrawable
    {
        public enum HitEventCountType
        {
            Total,
            DrumCentre,
            DrumRim,
        }

        [SettingSource("Unstable rate")]
        public Bindable<HitEventCountType> UnstableRateType { get; set; } =
            new Bindable<HitEventCountType>(HitEventCountType.Total);

        public bool UsesFixedAnchor { get; set; }

        protected override double RollingDuration => 0;

        private const float alpha_when_invalid = 0.3f;
        private readonly Bindable<bool> valid = new Bindable<bool>();

        [Resolved]
        private ScoreProcessor scoreProcessor { get; set; } = null!;

        public HitEventsCounter()
        {
            Current.Value = 0;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Colour = colours.BlueLighter;
            valid.BindValueChanged(e =>
                DrawableCount.FadeTo(e.NewValue ? 1 : alpha_when_invalid, 1000, Easing.OutQuint)
            );
            UnstableRateType.ValueChanged += _ => updateDisplay();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            scoreProcessor.NewJudgement += updateDisplay;
            scoreProcessor.JudgementReverted += updateDisplay;

            updateDisplay();
        }

        private void updateDisplay(JudgementResult result)
        {
            if (HitEventExtensions.AffectsUnstableRate(result.HitObject, result.Type))
                updateDisplay();
        }

        private void updateDisplay()
        {
            double hitCount = UnstableRateType.Value switch
            {
                HitEventCountType.DrumCentre => scoreProcessor.HitEvents.DrumCentreHitCount(),
                HitEventCountType.DrumRim => scoreProcessor.HitEvents.DrumRimHitCount(),
                _ => scoreProcessor.HitEvents.TotalDrumHitCount(),
            };

            valid.Value = hitCount != 0;

            if (hitCount != 0)
                Current.Value = (int)Math.Round(hitCount);
        }

        protected override IHasText CreateText() =>
            new TextComponent { Alpha = alpha_when_invalid, };

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (scoreProcessor.IsNotNull())
            {
                scoreProcessor.NewJudgement -= updateDisplay;
                scoreProcessor.JudgementReverted -= updateDisplay;
            }
        }

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
                        new OsuSpriteText
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Font = OsuFont.Numeric.With(size: 8, fixedWidth: true),
                            Text = @"hit events",
                            Padding = new MarginPadding { Bottom = 1.5f }, // align baseline better
                        }
                    }
                };
            }
        }
    }
}
