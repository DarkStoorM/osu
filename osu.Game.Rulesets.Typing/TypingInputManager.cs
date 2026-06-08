// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Typing
{
    public partial class TypingInputManager : RulesetInputManager<TypingAction>
    {
        public TypingInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.None) { }
    }

    public enum TypingAction
    {
        Q,

        W,

        E,

        R,

        T,

        Y,

        U,

        I,

        O,

        P,

        A,

        S,

        D,

        F,

        G,

        H,

        J,

        K,

        L,

        Z,

        X,

        C,

        V,

        B,

        N,

        M,

        Space
    }
}
