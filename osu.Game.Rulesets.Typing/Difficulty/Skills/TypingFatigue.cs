// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class TypingFatigue : StrainSkill
    {
        private double skillMultiplier => 1.45;
        private double strainDecayBase => 0.6;
        private double currentStrain;

        public TypingFatigue(Mod[] mods)
            : base(mods) { }

        protected override double StrainValueAt(DifficultyHitObject current)
        {
            TypingHitObject currentHitObject = (TypingHitObject)current.BaseObject;

            // 600 was an arbitrary value picked to make the typing fatigue factor climb steadily after 250~ objects
            // and approach 1.0 at ~1200+.
            // The factor is very small for the first ~150 objects to not favor short beatmaps
            double t = current.Index / 600.0;
            double fatigue = 1.0 - Math.Exp(-(t * t));

            // The typing fatigue increases with word length, but shouldn't explode the difficulty
            fatigue += 1 - Math.Pow(currentHitObject.IndexInWord, -0.1);

            currentStrain *= strainDecay(current.DeltaTime);
            currentStrain += fatigue * skillMultiplier;

            return currentStrain;
        }

        protected override double CalculateInitialStrain(double time, DifficultyHitObject current) => currentStrain * strainDecay(time - current.Previous(0).StartTime);

        private double strainDecay(double ms) => Math.Pow(strainDecayBase, ms / 1000);
    }
}
