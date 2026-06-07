// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Typing.Layouts.KeyboardData
{
    public static class PhysicalKeyExtensions
    {
        public static bool IsOnSameFinger(this PhysicalKey a, PhysicalKey b)
            => a.Hand == b.Hand &&
               a.Finger == b.Finger;

        public static bool IsOnSameHand(this PhysicalKey a, PhysicalKey b)
            => a.Hand == b.Hand;

        public static bool IsKeyRepeated(this PhysicalKey a, PhysicalKey b)
            => a.Character == b.Character;
    }
}
