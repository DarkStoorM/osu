// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Judgements;
using osu.Game.Rulesets.Typing.Scoring;

namespace osu.Game.Rulesets.Typing.Objects
{
    public class TypingHitObject : HitObject
    {
        public TypingAction Letter;

        public int IndexInWord { get; set; }

        public int WordLength { get; set; }

        public override Judgement CreateJudgement() => new TypingJudgement();

        protected override HitWindows CreateHitWindows() => new TypingHitWindows();
    }
}
