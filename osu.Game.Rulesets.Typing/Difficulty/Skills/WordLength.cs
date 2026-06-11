// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class WordLength : StrainDecaySkill
    {
        protected override double SkillMultiplier => 1;

        protected override double StrainDecayBase => 0.84;

        public WordLength(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            var currentObject = (TypingHitObject)current.BaseObject;

            double length = currentObject.WordLength;
            double index = currentObject.IndexInWord;

            if (length <= 0)
                return 0;

            double wordProgress = index / length;

            // Longer words should be disproportionately harder (I think?)
            // Also, technically, this could be capped above some length, but since the longest words exist in 5k/25k dictionaries
            // and are rare uncommon anyway, there is no need to do anything more here
            double lengthFactor = Math.Pow(length, 1.275);

            return lengthFactor * wordProgress;
        }
    }
}
