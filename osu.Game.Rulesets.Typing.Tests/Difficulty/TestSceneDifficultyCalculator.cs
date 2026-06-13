// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Typing.Difficulty;
using osu.Game.Rulesets.Typing.Tests.Difficulty.Components;
using osu.Game.Tests.Visual;
using Axes = osu.Framework.Graphics.Axes;
using Drawable = osu.Framework.Graphics.Drawable;
using MarginPadding = osu.Framework.Graphics.MarginPadding;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty
{
    [TestFixture]
    public partial class TestSceneDifficultyCalculator : OsuTestScene
    {
        private const double bpm = 100;
        private const double drain_time = 30000;

        public TestSceneDifficultyCalculator()
        {
            Beatmap beatmap = TypingDifficultyTestData.CreateBeatmap(bpm, drain_time);

            TypingDifficultyAttributes attributes = TypingDifficultyTestData.CalculateEnglish0K(beatmap);

            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(20),

                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.Relative, 0.1f),
                    new Dimension(GridSizeMode.Relative, 0.1f),
                    new Dimension(GridSizeMode.Relative, 0.8f),
                },

                Content = new[]
                {
                    new Drawable[]
                    {
                        new StatisticsBar(fontSize: 26,
                            $"Mod: English0K",
                            $"BPM: {bpm:F0}",
                            $"Length: {TimeSpan.FromMilliseconds(drain_time):mm\\:ss}",
                            $"HitObjects: {attributes.MaxCombo}",
                            $"Stars: {attributes.StarRating:F2}")
                    },
                    new Drawable[]
                    {
                        new StatisticsBar(fontSize: 16,
                            $"Finger Control:\n{attributes.FingerControl:F2}",
                            $"Key Travel:\n{attributes.KeyTravel:F2}",
                            $"Retrigger:\n{attributes.Retrigger:F2}",
                            $"Row Switch:\n{attributes.RowSwitch:F2}",
                            $"Speed:\n{attributes.Speed:F2}",
                            $"Typing Fatigue:\n{attributes.TypingFatigue:F2}",
                            $"Word Length:\n{attributes.WordLength:F2}"),
                    },
                    new Drawable[]
                    {
                        new GraphContainer(attributes.WordLengthSkill),
                    }
                }
            };
        }
    }
}
