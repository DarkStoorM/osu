// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Typing.Beatmaps;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Tests.Mods
{
    public partial class TestSceneTypingEnglish0K : TypingModTestScene
    {
        private const double base_beat_length = 60000 / 180;
        private const double beat_fourth = base_beat_length / 4;

        [Test]
        public void TestDefaultBeatmapTest() => CreateModTest(new ModTestData
        {
            Mod = new TypingModEnglish0K(),
            Autoplay = true,
            PassCondition = () => true,
            CreateBeatmap = () =>
            {
                var beatmap = new TypingBeatmap
                {
                    HitObjects = new List<TypingHitObject>
                    {
                        new TypingHitObject
                        {
                            Letter = TypingAction.A,
                            StartTime = 0,
                        },
                        new TypingHitObject
                        {
                            Letter = TypingAction.A,
                            StartTime = beat_fourth * 200,
                        },
                    },
                };

                beatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = base_beat_length });

                return beatmap;
            },
        });
    }
}
