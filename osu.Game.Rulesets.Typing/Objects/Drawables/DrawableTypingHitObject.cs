// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Typing.Objects.Drawables
{
    public partial class DrawableTypingHitObject : DrawableHitObject<TypingHitObject>, IKeyBindingHandler<TypingAction>
    {
        // Note: while this does not belong here and these colours are only used through the mod, this was the fastest
        // change to get this to work since I couldn't figure out how to do this directly inside the mod in
        // IApplicableToDrawableHitObject. HitObject is null at that point, so I moved this here. Not ideal, but this
        // will eventually be replaced with ruleset-specific settings
        private readonly Dictionary<Finger, Colour4> fingerColours = new Dictionary<Finger, Colour4>
        {
            // Blue-ish
            { Finger.Index, Color4Extensions.FromHex("#38628c77") },
            // Green-ish
            { Finger.Middle, Color4Extensions.FromHex("#43704877") },
            // Orange-ish
            { Finger.Ring, Color4Extensions.FromHex("#8d843f77") },
            // Red-ish
            { Finger.Pinky, Color4Extensions.FromHex("#88445377") },
        };

        private const float key_box_width = 90f;
        private const float key_box_height = 100f;
        private const float font_size = 100;

        private readonly Colour4 letterColor = Color4Extensions.FromHex("#CDD6F4");
        private readonly Colour4 keyColour = Color4Extensions.FromHex("#3c3c5a");

        private Box box = null!;
        private OsuSpriteText letterText = null!;
        private TypingAction? typingAction;

        /// <summary>
        /// When <see langword="true"/>, will change the color of the Box on this object to one that corresponds to the
        ///
        /// </summary>
        public bool OverrideKeyColor { get; set; }

        public DrawableTypingHitObject()
            : this(null) { }

        public DrawableTypingHitObject(TypingHitObject? hitObject)
            : base(hitObject) { }

        [BackgroundDependencyLoader]
        private void load()
        {
            Origin = Anchor.Centre;

            box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.White,
            };

            Container keyContainer = new Container
            {
                Size = new Vector2(key_box_width, key_box_height),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 10,
                Masking = true,
                Child = box
            };

            letterText = new OsuSpriteText
            {
                Font = OsuFont.Inter.With(size: font_size, weight: FontWeight.SemiBold),
                Colour = letterColor,
                Text = string.Empty,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            AddInternal(keyContainer);
            AddInternal(letterText);
        }

        protected override void OnApply()
        {
            base.OnApply();

            if (HitObject != null)
            {
                box.Colour = OverrideKeyColor
                    ? fingerColours[HitObject.CurrentKey.Finger]
                    : keyColour;

                letterText.Text = HitObject.Letter.ToString().ToUpperInvariant();

                // Those two glyphs are a bit too wide in the used font
                if (HitObject.Letter is TypingAction.W or TypingAction.M)
                    letterText.Scale = new Vector2(0.8f, 1f);
            }
            else
            {
                letterText.Text = string.Empty;
            }
        }

        protected override void OnFree()
        {
            base.OnFree();

            typingAction = null;
            Alpha = 1;
            ClearTransforms();
            letterText.Scale = Vector2.One;
            Position = Vector2.Zero;
        }

        public override IEnumerable<HitSampleInfo> GetSamples() => new[] { new HitSampleInfo(HitSampleInfo.HIT_NORMAL) };

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyMinResult();

                return;
            }

            HitResult result = HitObject.HitWindows.ResultFor(timeOffset);

            if (result == HitResult.None)
                return;

            if (typingAction == HitObject.Letter)
                ApplyResult(result);
            else
                ApplyMinResult();
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                    Alpha = 0;

                    break;

                case ArmedState.Miss:
                    const double duration = 500;

                    this.ScaleTo(0.5f, duration, Easing.OutQuint);
                    this.MoveToOffset(new Vector2(0, 75), duration, Easing.Out);
                    this.FadeColour(Color4.Red, duration / 2, Easing.OutQuint).Then().FadeOut(duration / 2, Easing.InQuint).Expire();

                    break;
            }
        }

        public bool OnPressed(KeyBindingPressEvent<TypingAction> e)
        {
            typingAction = e.Action;

            return UpdateResult(true);
        }

        public void OnReleased(KeyBindingReleaseEvent<TypingAction> e)
        {
            if (e.Action == typingAction)
                typingAction = null;
        }
    }
}
