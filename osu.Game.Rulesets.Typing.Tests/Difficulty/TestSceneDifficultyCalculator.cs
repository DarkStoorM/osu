// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty.Skills;
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
        private const double bpm = 150;
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
                            Array.Empty<Colour4>(),
                            $"Mod: English0K",
                            $"BPM: {bpm:F0}",
                            $"Length: {TimeSpan.FromMilliseconds(drain_time):mm\\:ss}",
                            $"HitObjects: {attributes.MaxCombo}",
                            $"Stars: {attributes.StarRating:F2}")
                    },
                    new Drawable[]
                    {
                        new StatisticsBar(fontSize: 16,
                            new[] { Colour4.IndianRed, Colour4.LightSteelBlue, Colour4.Green, Colour4.Yellow, Colour4.Violet, Colour4.Aqua },
                            $"Key Travel:\n{attributes.KeyTravel:F2}",
                            $"Retrigger:\n{attributes.Retrigger:F2}",
                            $"Row Switch:\n{attributes.RowSwitch:F2}",
                            $"Speed:\n{attributes.Speed:F2}",
                            $"Typing Fatigue:\n{attributes.TypingFatigue:F2}",
                            $"Word Length:\n{attributes.WordLength:F2}"),
                    },
                    new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Children = new[]
                            {
                                new GraphRowContainer(20,
                                    new[] { Colour4.IndianRed, Colour4.LightSteelBlue, Colour4.Green },
                                    attributes.KeyTravelSkill,
                                    attributes.RetriggerSkill,
                                    attributes.RowSwitchSkill),
                                new GraphRowContainer(0,
                                    new[] { Colour4.Yellow, Colour4.Violet, Colour4.Aqua },
                                    attributes.SpeedSkill,
                                    attributes.TypingFatigueSkill,
                                    attributes.WordLengthSkill),
                            }
                        }
                    }
                }
            };
        }

        private partial class GraphRowContainer : FillFlowContainer
        {
            public GraphRowContainer(float bottomMargin, Colour4[] colours, params StrainSkill[] attributes)
            {
                RelativeSizeAxes = Axes.Both;
                Direction = FillDirection.Horizontal;
                Width = 0.333f;
                Height = 0.475f;
                Margin = new MarginPadding { Bottom = bottomMargin };

                for (int i = 0; i < attributes.Length; i++)
                {
                    var attribute = attributes[i];
                    var colour = colours[i];

                    Add(new GraphContainer(attribute, colour));
                }
            }
        }
    }
}
