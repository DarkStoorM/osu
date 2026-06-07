// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public class TypingDifficultyHitObject : DifficultyHitObject
    {
        public TypingDifficultyHitObject(HitObject current, HitObject previous, double clockRate, List<DifficultyHitObject> allObjects, int index)
            : base(current, previous, clockRate, allObjects, index)
        {
            // Intentionally empty for now
        }
    }
}
