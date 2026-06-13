// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Typing.Difficulty.Skills;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public class TypingDifficultyAttributes : DifficultyAttributes
    {
        public double FingerControl { get; set; }
        public double KeyTravel { get; set; }
        public double Retrigger { get; set; }
        public double RowSwitch { get; set; }
        public double Speed { get; set; }
        public double TypingFatigue { get; set; }
        public double WordLength { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
        public FingerControl FingerControlSkill { get; set; }
        public KeyTravel KeyTravelSkill { get; set; }
        public Retrigger RetriggerSkill { get; set; }
        public RowSwitch RowSwitchSkill { get; set; }
        public Speed SpeedSkill { get; set; }
        public TypingFatigue TypingFatigueSkill { get; set; }
        public WordLength WordLengthSkill { get; set; }
    }
}
