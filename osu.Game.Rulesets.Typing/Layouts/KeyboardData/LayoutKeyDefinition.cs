// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Typing.Layouts.KeyboardData
{
    public readonly record struct LayoutKeyDefinition
    {
        public required TypingAction Character { get; init; }

        public required float X { get; init; }

        public required float Y { get; init; }

        public required Hand Hand { get; init; }

        public required Finger Finger { get; init; }

        public required KeyboardRow Row { get; init; }
    }
}
