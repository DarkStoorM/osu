// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;

namespace osu.Game.Rulesets.Typing.Layouts
{
    public sealed class QwertyOrtholinearLayout : KeyboardLayout
    {
        protected override IEnumerable<LayoutKeyDefinition> CreateLayout()
        {
            // Top Row
            yield return key(TypingAction.Q, 0.0f, 0, Hand.Left, Finger.Pinky, KeyboardRow.Top);
            yield return key(TypingAction.W, 1.0f, 0, Hand.Left, Finger.Ring, KeyboardRow.Top);
            yield return key(TypingAction.E, 2.0f, 0, Hand.Left, Finger.Middle, KeyboardRow.Top);
            yield return key(TypingAction.R, 3.0f, 0, Hand.Left, Finger.Index, KeyboardRow.Top);
            yield return key(TypingAction.T, 4.0f, 0, Hand.Left, Finger.Index, KeyboardRow.Top);

            yield return key(TypingAction.Y, 5.0f, 0, Hand.Right, Finger.Index, KeyboardRow.Top);
            yield return key(TypingAction.U, 6.0f, 0, Hand.Right, Finger.Index, KeyboardRow.Top);
            yield return key(TypingAction.I, 7.0f, 0, Hand.Right, Finger.Middle, KeyboardRow.Top);
            yield return key(TypingAction.O, 8.0f, 0, Hand.Right, Finger.Ring, KeyboardRow.Top);
            yield return key(TypingAction.P, 9.0f, 0, Hand.Right, Finger.Pinky, KeyboardRow.Top);

            // Home Row
            yield return key(TypingAction.A, 0.0f, 1, Hand.Left, Finger.Pinky, KeyboardRow.Home);
            yield return key(TypingAction.S, 1.0f, 1, Hand.Left, Finger.Ring, KeyboardRow.Home);
            yield return key(TypingAction.D, 2.0f, 1, Hand.Left, Finger.Middle, KeyboardRow.Home);
            yield return key(TypingAction.F, 3.0f, 1, Hand.Left, Finger.Index, KeyboardRow.Home);
            yield return key(TypingAction.G, 4.0f, 1, Hand.Left, Finger.Index, KeyboardRow.Home);

            yield return key(TypingAction.H, 5.0f, 1, Hand.Right, Finger.Index, KeyboardRow.Home);
            yield return key(TypingAction.J, 6.0f, 1, Hand.Right, Finger.Index, KeyboardRow.Home);
            yield return key(TypingAction.K, 7.0f, 1, Hand.Right, Finger.Middle, KeyboardRow.Home);
            yield return key(TypingAction.L, 8.0f, 1, Hand.Right, Finger.Ring, KeyboardRow.Home);

            // Bottom Row
            yield return key(TypingAction.Z, 0.0f, 2, Hand.Left, Finger.Pinky, KeyboardRow.Bottom);
            yield return key(TypingAction.X, 1.0f, 2, Hand.Left, Finger.Ring, KeyboardRow.Bottom);
            yield return key(TypingAction.C, 2.0f, 2, Hand.Left, Finger.Middle, KeyboardRow.Bottom);
            yield return key(TypingAction.V, 3.0f, 2, Hand.Left, Finger.Index, KeyboardRow.Bottom);
            yield return key(TypingAction.B, 4.0f, 2, Hand.Left, Finger.Index, KeyboardRow.Bottom);

            yield return key(TypingAction.N, 5.0f, 2, Hand.Right, Finger.Index, KeyboardRow.Bottom);
            yield return key(TypingAction.M, 6.0f, 2, Hand.Right, Finger.Index, KeyboardRow.Bottom);
        }

        private static LayoutKeyDefinition key(
            TypingAction character,
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
    }
}
