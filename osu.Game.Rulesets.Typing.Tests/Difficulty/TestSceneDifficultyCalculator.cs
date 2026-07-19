// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty.Skills;
using osu.Game.Rulesets.Typing.Difficulty;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Rulesets.Typing.Tests.Difficulty.Components;
using osu.Game.Tests.Visual;
using osuTK;
using Axes = osu.Framework.Graphics.Axes;
using Drawable = osu.Framework.Graphics.Drawable;
using MarginPadding = osu.Framework.Graphics.MarginPadding;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty
{
    public partial class TestSceneDifficultyCalculator : OsuTestScene
    {
        private const double bpm = 140;
        private double drainTime = 60000;
        private DictionarySize currentDictionarySize = DictionarySize.Curated;

        private Beatmap beatmap;
        private TypingDifficultyAttributes currentAttributes;
        private readonly StatisticsBar statisticsBar;
        private readonly StatisticsBar skillStatsBar;
        private readonly GraphRowContainer keyTravelRowContainer;
        private readonly GraphRowContainer speedRowContainer;

        public TestSceneDifficultyCalculator()
        {
            beatmap = TypingDifficultyTestData.CreateBeatmap(bpm, drainTime);
            currentAttributes = TypingDifficultyTestData.CalculateWithDictionarySize(beatmap, currentDictionarySize);

            var dictionarySizeOptions = new List<DictionarySize>
            {
                DictionarySize.Curated,
                DictionarySize.E0K,
                DictionarySize.E1K,
                DictionarySize.E5K
            };

            var dictionaryDropdown = new BasicDropdown<DictionarySize>
            {
                Items = dictionarySizeOptions,
                Current = { Value = DictionarySize.Curated },
                Width = 150
            };

            var beatmapLengthOptions = new List<(string label, double ms)>
            {
                ("10s", 10000),
                ("30s", 30000),
                ("60s", 60000),
                ("120s", 120000),
                ("180s", 180000)
            };

            var beatmapLengthDropdown = new BasicDropdown<(string label, double ms)>
            {
                Items = beatmapLengthOptions,
                Current = { Value = ("60s", 60000) },
                Width = 150
            };

            statisticsBar = new StatisticsBar(fontSize: 26,
                Array.Empty<Colour4>(),
                $"BPM: {bpm:F0}",
                $"Length: {TimeSpan.FromMilliseconds(drainTime):mm\\:ss}",
                $"HitObjects: {currentAttributes.MaxCombo}",
                $"Stars: {currentAttributes.StarRating:F2}");

            skillStatsBar = new StatisticsBar(fontSize: 16,
                new[] { Colour4.IndianRed, Colour4.LightSteelBlue, Colour4.Green, Colour4.Yellow, Colour4.Violet, Colour4.Aqua },
                $"Key Travel:\n{currentAttributes.KeyTravel:F2}",
                $"Retrigger:\n{currentAttributes.Retrigger:F2}",
                $"Row Switch:\n{currentAttributes.RowSwitch:F2}",
                $"Speed:\n{currentAttributes.Speed:F2}",
                $"Typing Fatigue:\n{currentAttributes.TypingFatigue:F2}",
                $"Word Length:\n{currentAttributes.WordLength:F2}");

            keyTravelRowContainer = new GraphRowContainer(20,
                new[] { Colour4.IndianRed, Colour4.LightSteelBlue, Colour4.Green },
                currentAttributes.KeyTravelSkill,
                currentAttributes.RetriggerSkill,
                currentAttributes.RowSwitchSkill);

            speedRowContainer = new GraphRowContainer(0,
                new[] { Colour4.Yellow, Colour4.Violet, Colour4.Aqua },
                currentAttributes.SpeedSkill,
                currentAttributes.TypingFatigueSkill,
                currentAttributes.WordLengthSkill);

            dictionaryDropdown.Current.BindValueChanged(OnDictionarySizeChanged);
            beatmapLengthDropdown.Current.BindValueChanged(OnBeatmapLengthChanged);

            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(20),

                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 50),
                    new Dimension(GridSizeMode.Relative, 0.08f),
                    new Dimension(GridSizeMode.Relative, 0.05f),
                    new Dimension(GridSizeMode.Relative, 0.8f),
                },

                Content = new[]
                {
                    new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(10, 0),
                            Children = new Drawable[]
                            {
                                dictionaryDropdown,
                                beatmapLengthDropdown
                            }
                        }
                    },
                    new Drawable[]
                    {
                        statisticsBar
                    },
                    new Drawable[]
                    {
                        skillStatsBar
                    },
                    new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Children = new Drawable[]
                            {
                                keyTravelRowContainer,
                                speedRowContainer,
                            }
                        }
                    }
                }
            };
        }

        private void OnDictionarySizeChanged(ValueChangedEvent<DictionarySize> e)
        {
            currentDictionarySize = e.NewValue;
            currentAttributes = TypingDifficultyTestData.CalculateWithDictionarySize(beatmap, e.NewValue);

            refreshStats();
        }

        private void OnBeatmapLengthChanged(ValueChangedEvent<(string label, double ms)> e)
        {
            drainTime = e.NewValue.ms;
            beatmap = TypingDifficultyTestData.CreateBeatmap(bpm, drainTime);
            currentAttributes = TypingDifficultyTestData.CalculateWithDictionarySize(beatmap, currentDictionarySize);

            refreshStats();
        }

        private void refreshStats()
        {
            statisticsBar.Content = new[]
            {
                new Drawable[]
                {
                    new InfoCell(26, $"BPM: {bpm:F0}", Colour4.White),
                    new InfoCell(26, $"Length: {TimeSpan.FromMilliseconds(drainTime):mm\\:ss}", Colour4.White),
                    new InfoCell(26, $"HitObjects: {currentAttributes.MaxCombo}", Colour4.White),
                    new InfoCell(26, $"Stars: {currentAttributes.StarRating:F2}", Colour4.White),
                }
            };

            skillStatsBar.Content = new[]
            {
                new Drawable[]
                {
                    new InfoCell(16, $"Key Travel:\n{currentAttributes.KeyTravel:F2}", Colour4.IndianRed),
                    new InfoCell(16, $"Retrigger:\n{currentAttributes.Retrigger:F2}", Colour4.LightSteelBlue),
                    new InfoCell(16, $"Row Switch:\n{currentAttributes.RowSwitch:F2}", Colour4.Green),
                    new InfoCell(16, $"Speed:\n{currentAttributes.Speed:F2}", Colour4.Yellow),
                    new InfoCell(16, $"Typing Fatigue:\n{currentAttributes.TypingFatigue:F2}", Colour4.Violet),
                    new InfoCell(16, $"Word Length:\n{currentAttributes.WordLength:F2}", Colour4.Aqua),
                }
            };

            keyTravelRowContainer.UpdateGraphs(
                new[] { Colour4.IndianRed, Colour4.LightSteelBlue, Colour4.Green },
                currentAttributes.KeyTravelSkill,
                currentAttributes.RetriggerSkill,
                currentAttributes.RowSwitchSkill);

            speedRowContainer.UpdateGraphs(
                new[] { Colour4.Yellow, Colour4.Violet, Colour4.Aqua },
                currentAttributes.SpeedSkill,
                currentAttributes.TypingFatigueSkill,
                currentAttributes.WordLengthSkill);
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

            public void UpdateGraphs(Colour4[] colours, params StrainSkill[] attributes)
            {
                Clear();

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
