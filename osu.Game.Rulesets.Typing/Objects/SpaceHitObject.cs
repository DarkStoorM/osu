// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Typing.Judgements;

namespace osu.Game.Rulesets.Typing.Objects
{
    /// <summary>
    /// HitObject that ONLY awards bonus score and doesn't contribute towards combo. Can be missed, but can
    /// cause note-locks when its HitWindow is ignored on lower accuracy plays.
    /// <para/>Note: this object always awards a fixed portion of score as bonus, no matter the HitResult.
    /// </summary>
    public class SpaceHitObject : TypingHitObject
    {
        public override Judgement CreateJudgement() => new SpaceJudgement();

        public SpaceHitObject() => Letter = TypingAction.Space;

        /*
         * Self note: the custom HitWindows could be added to the Space, e.g. harsher or fixed window of ~50ms,
         * but the issue with this would be that if you press space too late, you would miss the actual
         * HitObject since you would be entering the next HitWindow. It would probably be the best
         * to leave the default HitWindows of parent object and live with the note-lock for now
         */
    }
}
