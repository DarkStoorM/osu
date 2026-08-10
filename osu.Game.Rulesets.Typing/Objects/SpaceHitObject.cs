// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Typing.Judgements;

namespace osu.Game.Rulesets.Typing.Objects
{
    /// <summary>
    /// Optional HitObject that ONLY awards bonus score. Doesn't contribute towards combo.
    /// <para/>Note: this object always awards a fixed portion of score as bonus, no matter the HitResult.
    /// </summary>
    public class SpaceHitObject : TypingHitObject
    {
        public override Judgement CreateJudgement() => new SpaceJudgement();

        public SpaceHitObject() => Letter = TypingAction.Space;
    }
}
