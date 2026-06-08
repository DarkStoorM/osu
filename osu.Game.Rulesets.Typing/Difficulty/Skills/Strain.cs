// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osuTK;

namespace osu.Game.Rulesets.Typing.Difficulty.Skills
{
    public class Strain : StrainDecaySkill
    {
        private const double same_hand_penalty = 1.25;
        private const double cross_hand_penalty = 0.5; // cross-hand transitions are naturally easier
        private const double same_finger_penalty = 2;
        private const double repeated_key_penalty = 2.5;
        private const double per_pattern_index_penalty = 0.35;

        public Strain(Mod[] mods)
            : base(mods) { }

        protected override double SkillMultiplier => 0.5;
        protected override double StrainDecayBase => 0.75;

        protected override double StrainValueOf(DifficultyHitObject current)
        {
            DifficultyHitObject prev = current.Previous(0);

            if (prev == null)
                return 0;

            TypingDifficultyHitObject difficultyObject = (TypingDifficultyHitObject)current;

            PhysicalKey from = ((TypingDifficultyHitObject)prev).PhysicalKey;
            PhysicalKey to = difficultyObject.PhysicalKey;

            double movement = 1.0;

            if (to.IsOnSameHand(from))
            {
                // Distance only matters on the same hand
                double distance = Vector2.Distance(from.Position, to.Position);

                movement += distance * same_hand_penalty;

                // vertical movement is harder than horizontal
                movement += Math.Abs((int)from.Row - (int)to.Row) * rowModifier(to.Row);
            }
            else
            {
                movement *= cross_hand_penalty;
            }

            if (to.IsOnSameFinger(from))
                movement *= same_finger_penalty;

            if (to.IsKeyRepeated(from))
                movement *= repeated_key_penalty;

            movement *= fingerModifier(to.Finger) * objectSpacingModifier(difficultyObject) * (1 + per_pattern_index_penalty * difficultyObject.IndexInPattern);

            return movement;
        }

        private static double fingerModifier(Finger finger) => finger switch
        {
            Finger.Index => 1,
            Finger.Middle => 1.25,
            Finger.Ring => 1.5,
            Finger.Pinky => 1.75,
        };

        private static double rowModifier(KeyboardRow row) => row switch
        {
            KeyboardRow.Top => 1.5,
            KeyboardRow.Home => 1,
            KeyboardRow.Bottom => 2,
        };

        private double objectSpacingModifier(TypingDifficultyHitObject current)
        {
            // The strain penalty will be reduced if the spacing is greater than the next object, because there is more time for recovery
            // before the next object arrives. Normally, no need to do anything as the pattern index takes care of this
            if (current.NextObject == null)
                return 1;

            if (current.TimeToNext <= current.TimeFromPrevious)
                return 1;

            return current.TimeToNext > current.TimeFromPrevious ? -1 : 1;
        }
    }
}
