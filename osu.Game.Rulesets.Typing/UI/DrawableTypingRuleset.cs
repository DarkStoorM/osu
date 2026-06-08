// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Typing.Objects;
using osu.Game.Rulesets.Typing.Objects.Drawables;
using osu.Game.Rulesets.Typing.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Typing.UI
{
    [Cached]
    public partial class DrawableTypingRuleset : DrawableScrollingRuleset<TypingHitObject>
    {
        // This value comes from an arbitrary multiplier chosen from min/max ratio (1.35)
        // applied to the ComputeTimeRange() from taiko playfield adjustment container
        // to have a quick, hacky solution to ModConstantSpeed working in this ruleset
        private const double time_length_in_ms = 3500;

        public DrawableTypingRuleset(TypingRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            : base(ruleset, beatmap, mods)
        {
            Direction.Value = ScrollingDirection.Left;
        }

        protected override void Update()
        {
            base.Update();

            double multiplier = VisualisationMethod == ScrollVisualisationMethod.Constant
                ? Beatmap.BeatmapInfo.BPM * Beatmap.Difficulty.SliderMultiplier / 60
                : 1;

            TimeRange.Value = time_length_in_ms / multiplier;
        }

        protected override Playfield CreatePlayfield() => new TypingPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new TypingFramedReplayInputHandler(replay);

        public override DrawableHitObject<TypingHitObject> CreateDrawableRepresentation(TypingHitObject h) => new DrawableTypingHitObject(h);

        protected override PassThroughInputManager CreateInputManager() => new TypingInputManager(Ruleset.RulesetInfo);
    }
}
