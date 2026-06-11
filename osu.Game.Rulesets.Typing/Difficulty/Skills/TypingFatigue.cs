// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    /// <summary>
    /// Skill only applying a penalty based on how deep into the beatmap the player is.
    /// <para/> Note: This is not tweaked yet.
    /// </summary>
    public class TypingFatigue : StrainDecaySkill
    {
        private const double delta_strain_decay = 0.4;

        public TypingFatigue(Mod[] mods)
            : base(mods) { }

        protected override double SkillMultiplier => 1.125;

        // Note: due to tiny values, this has to be pretty large
        protected override double StrainDecayBase => 0.98;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            double strain = strainDecay(current.DeltaTime);

            // 600 was an arbitrary value picked to make the typing fatigue factor climb steadily after 250~ objects
            // and approach 1.0 at ~1200+.
            // The factor is very small for the first ~150 objects to not favor short beatmaps
            double t = current.Index / 600.0;
            double fatigue = 1.0 - Math.Exp(-(t * t));

            return strain * fatigue;
        }

        private double strainDecay(double ms) => Math.Pow(delta_strain_decay, ms / 1000);
    }
}
