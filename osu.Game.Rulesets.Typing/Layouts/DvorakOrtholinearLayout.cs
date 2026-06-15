// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;

namespace osu.Game.Rulesets.Typing.Layouts
{
    public sealed class DvorakOrtholinearLayout : KeyboardLayout
    {
        protected override IEnumerable<LayoutKeyDefinition> CreateLayout()
        {
            // Top Row
            yield return Key(TypingAction.P, 3, 0, Hand.Left, Finger.Index, KeyboardRow.Top);
            yield return Key(TypingAction.Y, 4, 0, Hand.Left, Finger.Index, KeyboardRow.Top);

            yield return Key(TypingAction.F, 5, 0, Hand.Right, Finger.Index, KeyboardRow.Top);
            yield return Key(TypingAction.G, 6, 0, Hand.Right, Finger.Index, KeyboardRow.Top);
            yield return Key(TypingAction.C, 7, 0, Hand.Right, Finger.Middle, KeyboardRow.Top);
            yield return Key(TypingAction.R, 8, 0, Hand.Right, Finger.Ring, KeyboardRow.Top);
            yield return Key(TypingAction.L, 9, 0, Hand.Right, Finger.Pinky, KeyboardRow.Top);

            // Home Row
            yield return Key(TypingAction.A, 0, 1, Hand.Left, Finger.Pinky, KeyboardRow.Home);
            yield return Key(TypingAction.O, 1, 1, Hand.Left, Finger.Ring, KeyboardRow.Home);
            yield return Key(TypingAction.E, 2, 1, Hand.Left, Finger.Middle, KeyboardRow.Home);
            yield return Key(TypingAction.U, 3, 1, Hand.Left, Finger.Index, KeyboardRow.Home);
            yield return Key(TypingAction.I, 4, 1, Hand.Left, Finger.Index, KeyboardRow.Home);

            yield return Key(TypingAction.D, 5, 1, Hand.Right, Finger.Index, KeyboardRow.Home);
            yield return Key(TypingAction.H, 6, 1, Hand.Right, Finger.Index, KeyboardRow.Home);
            yield return Key(TypingAction.T, 7, 1, Hand.Right, Finger.Middle, KeyboardRow.Home);
            yield return Key(TypingAction.N, 8, 1, Hand.Right, Finger.Ring, KeyboardRow.Home);
            yield return Key(TypingAction.S, 9, 1, Hand.Right, Finger.Pinky, KeyboardRow.Home);

            // Bottom Row
            yield return Key(TypingAction.Q, 1, 2, Hand.Left, Finger.Ring, KeyboardRow.Bottom);
            yield return Key(TypingAction.J, 2, 2, Hand.Left, Finger.Middle, KeyboardRow.Bottom);
            yield return Key(TypingAction.K, 3, 2, Hand.Left, Finger.Index, KeyboardRow.Bottom);
            yield return Key(TypingAction.X, 4, 2, Hand.Left, Finger.Index, KeyboardRow.Bottom);

            yield return Key(TypingAction.B, 5, 2, Hand.Right, Finger.Index, KeyboardRow.Bottom);
            yield return Key(TypingAction.M, 6, 2, Hand.Right, Finger.Index, KeyboardRow.Bottom);
            yield return Key(TypingAction.W, 7, 2, Hand.Right, Finger.Middle, KeyboardRow.Bottom);
            yield return Key(TypingAction.V, 8, 2, Hand.Right, Finger.Ring, KeyboardRow.Bottom);
            yield return Key(TypingAction.Z, 9, 2, Hand.Right, Finger.Pinky, KeyboardRow.Bottom);
        }
    }
}
