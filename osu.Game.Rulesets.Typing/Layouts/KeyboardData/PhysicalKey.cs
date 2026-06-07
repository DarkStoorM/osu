// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK;

namespace osu.Game.Rulesets.Typing.Layouts.KeyboardData
{
    public enum Hand
    {
        Left,
        Right
    }

    public enum Finger
    {
        Index,
        Middle,
        Ring,
        Pinky
    }

    public enum KeyboardRow
    {
        Top,
        Home,
        Bottom
    }

    public readonly record struct PhysicalKey
    {
        public required TypingAction Character { get; init; }

        public required Vector2 Position { get; init; }

        public required Hand Hand { get; init; }

        public required Finger Finger { get; init; }

        public required KeyboardRow Row { get; init; }
    }
}
