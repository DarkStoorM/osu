// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using osuTK;

namespace osu.Game.Rulesets.Typing.Layouts.KeyboardData
{
    public abstract class KeyboardLayout
    {
        public static readonly ImmutableDictionary<KeyboardLayoutType, KeyboardLayout> KEYBOARD_LAYOUTS = new Dictionary<KeyboardLayoutType, KeyboardLayout>
        {
            { KeyboardLayoutType.QwertyStaggered, new QwertyStaggeredLayout() },
            { KeyboardLayoutType.QwertyOrtholinear, new QwertyOrtholinearLayout() },
            { KeyboardLayoutType.DvorakStaggered, new DvorakStaggeredLayout() },
            { KeyboardLayoutType.DvorakOrtholinear, new DvorakOrtholinearLayout() },
            { KeyboardLayoutType.ColemakStaggered, new ColemakStaggeredLayout() },
            { KeyboardLayoutType.ColemakOrtholinear, new ColemakOrtholinearLayout() },
            { KeyboardLayoutType.ColemakDhStaggered, new ColemakDhStaggeredLayout() },
            { KeyboardLayoutType.ColemakDhOrtholinear, new ColemakDhOrtholinearLayout() },
        }.ToImmutableDictionary();

        private readonly Dictionary<TypingAction, PhysicalKey> keys;

        protected KeyboardLayout() => keys = CreateLayout().ToDictionary(key => key.Character, createPhysicalKey);

        public IReadOnlyDictionary<TypingAction, PhysicalKey> Keys => keys;

        public bool TryGetKey(TypingAction c, out PhysicalKey key) => keys.TryGetValue(c, out key);

        protected abstract IEnumerable<LayoutKeyDefinition> CreateLayout();

        protected static LayoutKeyDefinition Key(TypingAction character,
                                                 float x,
                                                 float y,
                                                 Hand hand,
                                                 Finger finger,
                                                 KeyboardRow row)
        {
            return new LayoutKeyDefinition
            {
                Character = character,
                X = x,
                Y = y,
                Hand = hand,
                Finger = finger,
                Row = row
            };
        }

        private static PhysicalKey createPhysicalKey(LayoutKeyDefinition definition)
        {
            return new PhysicalKey
            {
                Character = definition.Character,
                Position = new Vector2(definition.X, definition.Y),
                Hand = definition.Hand,
                Finger = definition.Finger,
                Row = definition.Row
            };
        }
    }
}
