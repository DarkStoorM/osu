// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Typing.Tests
{
    [TestFixture]
    public partial class TestSceneTypingPlayer : PlayerTestScene
    {
        protected override Ruleset CreatePlayerRuleset() => new TypingRuleset();
    }
}
