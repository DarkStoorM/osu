// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Taiko.Mods;
using osu.Game.Rulesets.Taiko.Objects;

namespace osu.Game.Rulesets.Taiko.Tests.Mods
{
    public partial class TestSceneTaikoModFullRandom : TaikoModTestScene
    {
        private double beatLength = 60000 / 180;
        private double beat1o4 => beatLength / 4;

        [Test]
        public void TestDefaultBeatmapTest() => CreateModTest(new ModTestData
        {
            Mod = new TaikoModFullRandom() { TurnKiaiIntoStreams = { Value = true } },
            Autoplay = true,
            PassCondition = () => true,
            CreateBeatmap = () =>
                {
                    double hit_time = 0;

                    var beatmap = new Beatmap<TaikoHitObject>
                    {
                        HitObjects = new List<TaikoHitObject>
                        {
                            new Hit
                            {
                                Type = HitType.Rim,
                                StartTime = hit_time,
                            },
                            new Hit
                            {
                                Type = HitType.Centre,
                                StartTime = beat1o4 * 200,
                            },
                        },
                    };

                    beatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = beatLength });

                    double time = 0;

                    addKiai(ref time, ref beatmap);
                    addKiai(ref time, ref beatmap);
                    addKiai(ref time, ref beatmap);

                    return beatmap;
                },
        });

        private void addKiai(ref double time, ref Beatmap<TaikoHitObject> beatmap)
        {
            time += beat1o4 * 2;
            beatmap.ControlPointInfo.Add(time, new EffectControlPoint { KiaiMode = true });
            time += beatLength * 10;
            beatmap.ControlPointInfo.Add(time, new EffectControlPoint { KiaiMode = false });
        }
    }
}
