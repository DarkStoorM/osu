// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;

namespace osu.Game.Rulesets.Typing.Layouts
{
    public sealed class ColemakDhOrtholinearLayout : KeyboardLayout
    {
        protected override IEnumerable<LayoutKeyDefinition> CreateLayout()
        {
            // Top Row
            yield return Key(TypingAction.Q, 0, 0, Hand.Left, Finger.Pinky, KeyboardRow.Top);
            yield return Key(TypingAction.W, 1, 0, Hand.Left, Finger.Ring, KeyboardRow.Top);
            yield return Key(TypingAction.F, 2, 0, Hand.Left, Finger.Middle, KeyboardRow.Top);
            yield return Key(TypingAction.P, 3, 0, Hand.Left, Finger.Index, KeyboardRow.Top);
            yield return Key(TypingAction.B, 4, 0, Hand.Left, Finger.Index, KeyboardRow.Top);

            yield return Key(TypingAction.J, 5, 0, Hand.Right, Finger.Index, KeyboardRow.Top);
            yield return Key(TypingAction.L, 6, 0, Hand.Right, Finger.Index, KeyboardRow.Top);
            yield return Key(TypingAction.U, 7, 0, Hand.Right, Finger.Middle, KeyboardRow.Top);
            yield return Key(TypingAction.Y, 8, 0, Hand.Right, Finger.Ring, KeyboardRow.Top);

            // Home Row
            yield return Key(TypingAction.A, 0, 1, Hand.Left, Finger.Pinky, KeyboardRow.Home);
            yield return Key(TypingAction.R, 1, 1, Hand.Left, Finger.Ring, KeyboardRow.Home);
            yield return Key(TypingAction.S, 2, 1, Hand.Left, Finger.Middle, KeyboardRow.Home);
            yield return Key(TypingAction.T, 3, 1, Hand.Left, Finger.Index, KeyboardRow.Home);
            yield return Key(TypingAction.G, 4, 1, Hand.Left, Finger.Index, KeyboardRow.Home);

            yield return Key(TypingAction.M, 5, 1, Hand.Right, Finger.Index, KeyboardRow.Home);
            yield return Key(TypingAction.N, 6, 1, Hand.Right, Finger.Index, KeyboardRow.Home);
            yield return Key(TypingAction.E, 7, 1, Hand.Right, Finger.Middle, KeyboardRow.Home);
            yield return Key(TypingAction.I, 8, 1, Hand.Right, Finger.Ring, KeyboardRow.Home);
            yield return Key(TypingAction.O, 9, 1, Hand.Right, Finger.Pinky, KeyboardRow.Home);

            // Bottom Row
            yield return Key(TypingAction.X, 0, 2, Hand.Left, Finger.Pinky, KeyboardRow.Bottom);
            yield return Key(TypingAction.C, 1, 2, Hand.Left, Finger.Ring, KeyboardRow.Bottom);
            yield return Key(TypingAction.D, 2, 2, Hand.Left, Finger.Middle, KeyboardRow.Bottom);
            yield return Key(TypingAction.V, 3, 2, Hand.Left, Finger.Index, KeyboardRow.Bottom);
            yield return Key(TypingAction.Z, 4, 2, Hand.Left, Finger.Index, KeyboardRow.Bottom);

            yield return Key(TypingAction.K, 5, 2, Hand.Right, Finger.Index, KeyboardRow.Bottom);
            yield return Key(TypingAction.H, 6, 2, Hand.Right, Finger.Index, KeyboardRow.Bottom);
        }
    }
}
