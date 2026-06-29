// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Typing.Difficulty;
using osu.Game.Rulesets.Typing.Mods;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty
{
    public static class TypingDifficultyTestData
    {
        public static Beatmap CreateBeatmap(double bpm, double timeInMs)
        {
            Beatmap beatmap = new Beatmap
            {
                HitObjects = new List<HitObject>
                {
                    new HitObject { StartTime = 0 },
                    new HitObject { StartTime = timeInMs }
                }
            };

            beatmap.ControlPointInfo.Add(0, new TimingControlPoint
            {
                BeatLength = 60000 / bpm
            });

            return beatmap;
        }

        public static TypingDifficultyAttributes CalculateEnglish0K(
            Beatmap beatmap,
            int seed = 1,
            BeatLength beatLength = BeatLength.Half)
        {
            var mod = new TypingWordsMod
            {
                Seed = { Value = seed },
                AdjustBeatLength = { Value = beatLength }
            };

            FlatWorkingBeatmap working = new FlatWorkingBeatmap(beatmap);

            TypingRuleset ruleset = new TypingRuleset();

            var calculator = ruleset.CreateDifficultyCalculator(working);

            return (TypingDifficultyAttributes)calculator.Calculate(new Mod[] { mod });
        }
    }
}
