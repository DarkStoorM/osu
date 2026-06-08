// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Difficulty;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public class TypingDifficultyAttributes : DifficultyAttributes
    {
        public double Strain { get; set; }
        public double Speed { get; set; }
        public double Retrigger { get; set; }
    }
}
