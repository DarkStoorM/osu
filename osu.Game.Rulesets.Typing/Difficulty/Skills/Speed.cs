// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class Speed : StrainDecaySkill
    {
        public Speed(Mod[] mods)
            : base(mods) { }

        protected override double SkillMultiplier => 0.5;

        protected override double StrainDecayBase => 0.3;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            if (current.Previous(0) == null)
                return 0;

            var curr = (TypingDifficultyHitObject)current;

            double delta = Math.Max(40, curr.TimeFromPrevious);
            double speed = 1000.0 / delta;
            double continuity = 1.0;
            double ratio = curr.TimeFromPrevious / Math.Max(1, curr.TimeToNext);

            // stable rhythm = harder stream
            continuity += 1.0 - Math.Min(1.0, Math.Abs(1 - ratio));
            continuity += curr.IndexInPattern * 0.5;

            return speed * continuity;
        }
    }
}
