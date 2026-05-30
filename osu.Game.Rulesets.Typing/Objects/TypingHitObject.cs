// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Typing.Objects
{
    public class TypingHitObject : HitObject
    {
        public TypingAction Letter;

        public override Judgement CreateJudgement() => new Judgement();
    }
}
