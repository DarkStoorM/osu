// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Typing.Replays
{
    public class TypingReplayFrame : ReplayFrame
    {
        public List<TypingAction> Actions = new List<TypingAction>();

        public TypingReplayFrame(TypingAction? button = null)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }

        public override bool IsEquivalentTo(ReplayFrame other)
            => other is TypingReplayFrame typingFrame && Time == typingFrame.Time && Actions.SequenceEqual(typingFrame.Actions);
    }
}
