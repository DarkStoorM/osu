// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
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
        public DrawableTypingRuleset(TypingRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            : base(ruleset, beatmap, mods)
        {
            Direction.Value = ScrollingDirection.Left;
            TimeRange.Value = 6000;
        }

        protected override Playfield CreatePlayfield() => new TypingPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new TypingFramedReplayInputHandler(replay);

        public override DrawableHitObject<TypingHitObject> CreateDrawableRepresentation(TypingHitObject h) => new DrawableTypingHitObject(h);

        protected override PassThroughInputManager CreateInputManager() => new TypingInputManager(Ruleset.RulesetInfo);
    }
}
