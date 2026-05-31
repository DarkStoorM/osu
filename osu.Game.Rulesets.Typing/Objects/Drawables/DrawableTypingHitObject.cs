// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Typing.Objects.Drawables
{
    public partial class DrawableTypingHitObject : DrawableHitObject<TypingHitObject>, IKeyBindingHandler<TypingAction>
    {
        private const float hit_object_width = 70;
        private const float hit_object_height = 100;
        private const float hit_object_judgment_line_width = 2;
        private const float font_size = 90;

        private readonly Color4 hitObjectBoxColour = new Colour4(50, 50, 50, 255);
        private readonly Color4 hitObjectJudgmentColour = new Colour4(255, 255, 255, 50);

        public DrawableTypingHitObject(TypingHitObject hitObject)
            : base(hitObject) { }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(hit_object_width, hit_object_height);
            Origin = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                new Box
                {
                    Colour = hitObjectBoxColour,
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },

                new Box
                {
                    Colour = hitObjectJudgmentColour,
                    Size = new Vector2(hit_object_judgment_line_width, hit_object_height),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },

                new OsuSpriteText
                {
                    Font = OsuFont.Inter.With(size: font_size, fixedWidth: true),
                    Text = HitObject.Letter.ToString().ToUpperInvariant(),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            });
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

            if (TypingAction == HitObject.Letter)
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
            TypingAction = e.Action;

            return UpdateResult(true);
        }

        public TypingAction? TypingAction { get; private set; }

        public void OnReleased(KeyBindingReleaseEvent<TypingAction> e)
        {
            if (e.Action == TypingAction)
                TypingAction = null;
        }
    }
}
