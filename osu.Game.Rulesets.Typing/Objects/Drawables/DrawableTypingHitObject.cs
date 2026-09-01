// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
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
        private const string space_text_character = "|";

        // Note: while this does not belong here and these colours are only used through the mod, this was the fastest
        // change to get this to work since I couldn't figure out how to do this directly inside the mod in
        // IApplicableToDrawableHitObject. HitObject is null at that point, so I moved this here. Not ideal, but this
        // will eventually be replaced with ruleset-specific settings
        private readonly Dictionary<Finger, Colour4> fingerColours = new Dictionary<Finger, Colour4>();

        private const float font_size = 100;

        private readonly Colour4 defaultLetterColour = Color4Extensions.FromHex("#CDD6F4");
        private Colour4 spaceColour;

        private OsuSpriteText letterText = null!;
        private TypingAction? typingAction;

        /// <summary>
        /// When <see langword="true"/>, will change the colour of the letter on this object to one that corresponds to
        /// the finger that should theoretically be used for this object based on touch-typing layouts.
        /// </summary>
        public bool OverrideKeyColour { get; set; }

        public DrawableTypingHitObject()
            : this(null) { }

        public DrawableTypingHitObject(TypingHitObject? hitObject)
            // Note: this is a naive null suppression to silence a warning, and this exists here because of the
            // Pool registration from TypingPlayfield
            : base(hitObject!) { }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            fingerColours.Add(Finger.Index, colours.Blue1);
            fingerColours.Add(Finger.Middle, colours.Green2);
            fingerColours.Add(Finger.Ring, colours.YellowDark);
            fingerColours.Add(Finger.Pinky, colours.Red1);

            spaceColour = colours.Purple1;

            Origin = Anchor.Centre;

            letterText = new OsuSpriteText
            {
                Font = OsuFont.Inter.With(size: font_size, weight: FontWeight.SemiBold),
                Colour = defaultLetterColour,
                Text = string.Empty,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            AddInternal(letterText);
        }

        protected override void OnApply()
        {
            base.OnApply();

            if (HitObject is SpaceHitObject)
            {
                // This is very specific only to the current object that represents a space, which is the Pipe|Character
                // so it appears as a word separator vertically across the playfield. Change this accordingly if a
                // different character is used
                if (OverrideKeyColour)
                    letterText.Colour = spaceColour;

                letterText.Text = space_text_character;
                letterText.Scale = new Vector2(2, 2);
            }
            else
            {
                letterText.Colour = OverrideKeyColour
                    ? fingerColours[HitObject.CurrentKey.Finger]
                    : defaultLetterColour;

                letterText.Text = HitObject.Letter.ToString().ToUpperInvariant();
            }
        }

        protected override void OnFree()
        {
            base.OnFree();

            typingAction = null;
            Alpha = 1;
            ClearTransforms();
            letterText.Alpha = 1;
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

            // Self note: space key does not belong here and should in fact be its own DHO, but I don't think I care enough
            // about this right now. I will revisit this some day. Same thing about armed miss state
            if (HitObject is SpaceHitObject && typingAction == TypingAction.Space)
                ApplyResult(HitResult.LargeBonus);
            else if (HitObject is not SpaceHitObject && typingAction == HitObject.Letter)
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
                    // We don't really care what happens when we miss the bonus object
                    if (HitObject is SpaceHitObject)
                    {
                        Alpha = 0;

                        Expire();

                        break;
                    }

                    const double duration = 500;

                    // If the player selects Finger Guide mod, the missed letter will try to fade from its current colour
                    // to Black instead if not reset
                    letterText.Colour = defaultLetterColour;

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
