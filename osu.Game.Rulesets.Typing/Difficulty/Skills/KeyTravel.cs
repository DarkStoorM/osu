// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class KeyTravel : StrainDecaySkill
    {
        public KeyTravel(Mod[] mods)
            : base(mods) { }

        protected override double SkillMultiplier => 1;
        protected override double StrainDecayBase => 1;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            return 1;
        }
    }
}
