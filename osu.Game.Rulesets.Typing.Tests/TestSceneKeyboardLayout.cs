// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Utils;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Rulesets.Typing.Objects;
using osu.Game.Rulesets.Typing.Screens.Ranking.Statistics;
using osu.Game.Scoring;
using osu.Game.Tests.Visual;
using osuTK;

namespace osu.Game.Rulesets.Typing.Tests
{
    [TestFixture]
    public partial class TestSceneKeyboardLayout : OsuTestScene
    {
        public TestSceneKeyboardLayout()
        {
            var score = new ScoreInfo();

            var hitEvents = new List<HitEvent>();

            for (int i = 0; i < 500; i++)
            {
                TypingAction[] keys = Enum.GetValues<TypingAction>();
                TypingHitObject hitObject = new TypingHitObject { Letter = keys[RNG.Next(0, keys.Length)] };

                hitEvents.Add(new HitEvent(RNG.Next(0, 70), 1D, HitResult.Great, hitObject, hitObject, new Vector2(0, 0)));
            }

            score.HitEvents = hitEvents;
            score.Mods = new Mod[] { new TypingWordsMod() };

            Child = new KeyTimingDistribution(score);
        }
    }
}
