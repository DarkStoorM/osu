// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Typing.Mods
{
    public abstract class TypingMod : Mod, IHasSeed
    {
        [SettingSource("Word Seed", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> Seed { get; } = new Bindable<int?>();

        protected Random ModRNG = null!;

        protected static TypingAction LetterToTypingAction(char c)
        {
            // Mehhhhh
            switch (c)
            {
                case 'q': return TypingAction.Q;

                case 'w': return TypingAction.W;

                case 'e': return TypingAction.E;

                case 'r': return TypingAction.R;

                case 't': return TypingAction.T;

                case 'y': return TypingAction.Y;

                case 'u': return TypingAction.U;

                case 'i': return TypingAction.I;

                case 'o': return TypingAction.O;

                case 'p': return TypingAction.P;

                case 'a': return TypingAction.A;

                case 's': return TypingAction.S;

                case 'd': return TypingAction.D;

                case 'f': return TypingAction.F;

                case 'g': return TypingAction.G;

                case 'h': return TypingAction.H;

                case 'j': return TypingAction.J;

                case 'k': return TypingAction.K;

                case 'l': return TypingAction.L;

                case 'z': return TypingAction.Z;

                case 'x': return TypingAction.X;

                case 'c': return TypingAction.C;

                case 'v': return TypingAction.V;

                case 'b': return TypingAction.B;

                case 'n': return TypingAction.N;

                case 'm': return TypingAction.M;

                // This should catch some jank in the dictionaries
                default: throw new ArgumentOutOfRangeException(nameof(c), c, null);
            }
        }
    }
}
