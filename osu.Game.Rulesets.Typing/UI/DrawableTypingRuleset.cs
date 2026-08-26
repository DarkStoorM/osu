// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Typing.Configuration;
using osu.Game.Rulesets.Typing.Objects;
using osu.Game.Rulesets.Typing.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Typing.UI
{
    [Cached]
    public partial class DrawableTypingRuleset : DrawableScrollingRuleset<TypingHitObject>
    {
        public const double DEFAULT_SCROLL_TIME = 3500.0;
        public const double SCROLL_TIME_STEP = 100.0;

        public const double MIN_SCROLL_TIME = 1000.0;
        public const double MAX_SCROLL_TIME = 11000.0;

        public const double MAX_SCROLL_ADJUSTMENT_AMOUNT = 100.0;

        // This should be responsible only for encoding the replay frames, e.g. time + TypingAction,
        // and probably without the versions/headers since this will MOST LIKELY never change. Header
        // I guess would also not be required, there's probably no other purpose in a custom ruleset
        // other than storing a short info about what kind of ruleset the replay belongs to.
        // private readonly TypingReplaySerialiser replaySerialiser;
        //
        // This should only be responsible for persisting the replay in Realm, keeping in mind the
        // important fact that ScoreInfo might not exist yet while trying to persist and associate the
        // replay with certain Score.
        // private readonly TypingReplayRealmStorage replayRealmStorage;

        private double timeLengthInMs = DEFAULT_SCROLL_TIME;

        public DrawableTypingRuleset(TypingRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            : base(ruleset, beatmap, mods)
        {
            Direction.Value = ScrollingDirection.Left;
        }

        private readonly BindableDouble configScrollTime = new BindableDouble();
        private readonly BindableDouble configScrollAdjustmentCount = new BindableDouble();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (Config is not TypingRulesetConfigManager typingConfig)
                return;

            typingConfig.BindWith(TypingRulesetSetting.ScrollTime, configScrollTime);
            typingConfig.BindWith(TypingRulesetSetting.ScrollAdjustmentCount, configScrollAdjustmentCount);
            configScrollTime.BindValueChanged(v => timeLengthInMs = v.NewValue, true);
        }

        protected override void AdjustScrollSpeed(int amount)
        {
            if (!AllowScrollSpeedAdjustment)
                return;

            double oldVal = configScrollTime.Value;
            double newVal = Math.Clamp(configScrollTime.Value - amount * SCROLL_TIME_STEP, MIN_SCROLL_TIME, MAX_SCROLL_TIME);

            configScrollTime.Value = newVal;

            if (newVal != oldVal)
                configScrollAdjustmentCount.Value -= amount;

            timeLengthInMs = newVal;
        }

        protected override void Update()
        {
            base.Update();

            double multiplier = VisualisationMethod == ScrollVisualisationMethod.Constant
                ? Beatmap.BeatmapInfo.BPM * Beatmap.Difficulty.SliderMultiplier / 60
                : 1;

            TimeRange.Value = timeLengthInMs / multiplier;
        }

        protected override Playfield CreatePlayfield() => new TypingPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new TypingFramedReplayInputHandler(replay);

        // Note: Intentionally left here so I don't forget what I needed to do
        public override void SetReplayScore(Score replayScore) => base.SetReplayScore(replayScore);

        protected override ReplayRecorder CreateReplayRecorder(Score score) => new TypingReplayRecorder(score);

        public override DrawableHitObject<TypingHitObject>? CreateDrawableRepresentation(TypingHitObject h) => null;

        protected override PassThroughInputManager CreateInputManager() => new TypingInputManager(Ruleset.RulesetInfo);
    }
}
