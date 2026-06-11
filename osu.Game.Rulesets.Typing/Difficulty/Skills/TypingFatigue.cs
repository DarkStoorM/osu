// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class TypingFatigue : StrainDecaySkill
    {
        private const double delta_strain_decay = 0.5;

        public TypingFatigue(Mod[] mods)
            : base(mods) { }

        protected override double SkillMultiplier => 1;

        protected override double StrainDecayBase => 0.9;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            double strain = strainDecay(current.DeltaTime);

            // 800 Was an arbitrary value picked to make the typing fatigue factor climb to 0.5 at around 700~ objects
            // and approach 1.0 at ~2000+
            double t = current.Index / 800.0;
            double fatigue = 1.0 - Math.Exp(-(t * t)); // Short from Pow

            return strain * fatigue;
        }

        private double strainDecay(double ms) => Math.Pow(delta_strain_decay, ms / 1000);
    }
}
