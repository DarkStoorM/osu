// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Beatmaps;
using osu.Game.Rulesets.Typing.Configuration;
using osu.Game.Rulesets.Typing.Difficulty;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Rulesets.Typing.Scoring;
using osu.Game.Rulesets.Typing.Screens.Ranking.Statistics;
using osu.Game.Rulesets.Typing.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Statistics;
using osu.Game.Utils;

namespace osu.Game.Rulesets.Typing
{
    public class TypingRuleset : Ruleset
    {
        public const string SHORT_NAME = "typing";

        public override string Description => $"osu!{SHORT_NAME}";
        public override string ShortName => SHORT_NAME;

        public override IRulesetConfigManager CreateConfig(SettingsStore? settings) => new TypingRulesetConfigManager(settings, RulesetInfo);

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods = null) => new DrawableTypingRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new TypingBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new TypingDifficultyCalculator(RulesetInfo, beatmap);

        public override HealthProcessor CreateHealthProcessor(double drainStartTime) => new TypingHealthProcessor();

        public override ScoreProcessor CreateScoreProcessor() => new TypingScoreProcessor();

        public override ScoreMultiplierCalculator CreateScoreMultiplierCalculator(ScoreMultiplierContext context) => new TypingScoreMultiplierCalculator(context);

        public static Dictionary<DictionarySize, WeightedRandomWordGenerator> WordDictionaries { get; private set; } = new Dictionary<DictionarySize, WeightedRandomWordGenerator>();

        public TypingRuleset()
        {
            // Note: ruleset seems to be instantiated every time a beatmapset is selected, so the dictionaries should only be created once
            if (WordDictionaries.Count != 0)
                return;

            var resources = new ResourceStore<byte[]>(new DllResourceStore(typeof(TypingRuleset).Assembly));

            WordDictionaries = WordDictionaryProvider.CreateDictionaries(resources);
        }

        public override IResourceStore<byte[]> CreateResourceStore() => new DllResourceStore(typeof(TypingRuleset).Assembly);

        public override IEnumerable<HitResult> GetValidHitResults()
        {
            return new[]
            {
                HitResult.Miss,
                HitResult.Meh,
                HitResult.Ok,
                HitResult.Good,
                HitResult.Great,
                HitResult.Perfect,
                HitResult.LargeBonus
            };
        }

        public override LocalisableString GetDisplayNameForHitResult(HitResult result)
        {
            // The only hit result that needs a name is the Space really, because otherwise it would be confusing where
            // the "LargeBonus" comes from
            if (result == HitResult.LargeBonus)
                return "Spaces";

            return base.GetDisplayNameForHitResult(result);
        }

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            // Note: for this ruleset, the only extra mods I decided to migrate are: NF, HT, DT, DA, CS (auto is self-explanatory).
            // The reason for this was to have a fine control through rate change, and better control of the
            // Accuracy and Scroll Speed. No other mods are necessary. DA replaces HR/EZ anyway
            switch (type)
            {
                case ModType.Automation:
                    return new Mod[]
                    {
                        new TypingModAutoplay(),
                    };

                case ModType.DifficultyReduction:
                    return new Mod[]
                    {
                        new TypingModNoFail(),
                        new TypingModHalfTime(),
                    };

                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new TypingModDoubleTime(),
                    };

                case ModType.Conversion:
                    return new Mod[]
                    {
                        new TypingModDifficultyAdjust(),
                        // The reason for CS to exist here is to remove all speed changes from control points
                        new TypingModConstantSpeed(),
                        new TypingModWords(),
                    };

                case ModType.System:
                    return new Mod[]
                    {
                        new TypingModFingerGuide(),
                    };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override StatisticItem[] CreateStatisticsForScore(ScoreInfo score, IBeatmap playableBeatmap)
        {
            return new[]
            {
                new StatisticItem("Key timing distribution", () => new KeyTimingDistribution(score.HitEvents, score.Mods), requiresHitEvents: true)
            };
        }

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) =>
            new[]
            {
                new KeyBinding(InputKey.Q, TypingAction.Q),
                new KeyBinding(InputKey.W, TypingAction.W),
                new KeyBinding(InputKey.E, TypingAction.E),
                new KeyBinding(InputKey.R, TypingAction.R),
                new KeyBinding(InputKey.T, TypingAction.T),
                new KeyBinding(InputKey.Y, TypingAction.Y),
                new KeyBinding(InputKey.U, TypingAction.U),
                new KeyBinding(InputKey.I, TypingAction.I),
                new KeyBinding(InputKey.O, TypingAction.O),
                new KeyBinding(InputKey.P, TypingAction.P),
                new KeyBinding(InputKey.A, TypingAction.A),
                new KeyBinding(InputKey.S, TypingAction.S),
                new KeyBinding(InputKey.D, TypingAction.D),
                new KeyBinding(InputKey.F, TypingAction.F),
                new KeyBinding(InputKey.G, TypingAction.G),
                new KeyBinding(InputKey.H, TypingAction.H),
                new KeyBinding(InputKey.J, TypingAction.J),
                new KeyBinding(InputKey.K, TypingAction.K),
                new KeyBinding(InputKey.L, TypingAction.L),
                new KeyBinding(InputKey.Z, TypingAction.Z),
                new KeyBinding(InputKey.X, TypingAction.X),
                new KeyBinding(InputKey.C, TypingAction.C),
                new KeyBinding(InputKey.V, TypingAction.V),
                new KeyBinding(InputKey.B, TypingAction.B),
                new KeyBinding(InputKey.N, TypingAction.N),
                new KeyBinding(InputKey.M, TypingAction.M),
                new KeyBinding(InputKey.Space, TypingAction.Space),
            };

        // Self note: I know this is messy, but it will do for now
        public override IEnumerable<RulesetBeatmapAttribute> GetBeatmapAttributesForDisplay(IBeatmapInfo beatmapInfo, IReadOnlyCollection<Mod> mods)
        {
            TypingModWords? typingModWords = mods.OfType<TypingModWords>().FirstOrDefault();

            // Nothing else to do here if the mod was not selected
            if (typingModWords == null)
                return Array.Empty<RulesetBeatmapAttribute>();

            // The Words mod has customisation for Letter Spacing, which can generate letters at double or half the beat length,
            // which naturally affects the WPM. This will make the changes reflect when customising the mod
            double modBeatDivisor = typingModWords.LetterSpacing.Value switch
            {
                LetterSpacing.Narrow => 2.0,
                LetterSpacing.Wide => 0.5,
                _ => 1.0
            };

            const double max_score = ScoreProcessor.MAX_SCORE;
            var calculator = new TypingScoreMultiplierCalculator(new ScoreMultiplierContext(beatmapInfo.Difficulty));
            double scoreMultiplier = calculator.CalculateFor(mods);

            double rate = ModUtils.CalculateRateWithMods(mods);
            var adjustedDifficulty = GetAdjustedDisplayDifficulty(beatmapInfo, mods);
            var colours = new OsuColour();
            var hitWindows = new TypingHitWindows();

            hitWindows.SetDifficulty(adjustedDifficulty.OverallDifficulty);

            // In general, if we treat 5 letters as a standard length of full word on average, we can assume that one word
            // also consists of five beats (every letter is considered beat).
            // WPM can be calculated from [WPM = BPM / (5 * spacing)], spacing is further adjusted by Letter Spacing customisation.
            // The 2.5 value is a result of five required beats per word divided by two, since, by default, letters
            // are spaced by 1/2. Then, this value is adjusted to 1.25 for Narrow Spacing, 5 for Wide
            const double wpm_beat_factor = 2.5;
            double bpmAdjusted = beatmapInfo.BPM * rate;

            double wpm = bpmAdjusted / wpm_beat_factor;
            double wpmAdjusted = bpmAdjusted / (wpm_beat_factor / modBeatDivisor);

            // Mod multiplier also affects bonus score, because the total consists of regular results + bonus results
            double bonusSpacesScore = typingModWords.AddBonusSpaceHitObjects.Value
                ? max_score * TypingScoreProcessor.BONUS_SPACE_SCORE_FRACTION * scoreMultiplier
                : 0;

            double scoreWithModMultiplier = max_score * scoreMultiplier;
            double odMultiplier = TypingScoreProcessor.CalculateOverallDifficultyMultiplier(adjustedDifficulty.OverallDifficulty);
            double od10Multiplier = TypingScoreProcessor.CalculateOverallDifficultyMultiplier(10);
            double adjustedScore = Math.Round(scoreWithModMultiplier * odMultiplier + bonusSpacesScore);
            double maxPossibleScore = scoreWithModMultiplier * od10Multiplier + bonusSpacesScore;

            List<RulesetBeatmapAttribute.AdditionalMetric> additionalMetrics = new List<RulesetBeatmapAttribute.AdditionalMetric>();

            if (typingModWords.LetterSpacing.Value != LetterSpacing.Default)
                additionalMetrics.Add(new RulesetBeatmapAttribute.AdditionalMetric("Letter Spacing Score", $"{scoreWithModMultiplier - max_score:N0}"));

            if (Math.Abs(adjustedDifficulty.OverallDifficulty - 5) > double.Epsilon)
                additionalMetrics.Add(new RulesetBeatmapAttribute.AdditionalMetric("Overall Difficulty Score", $"{max_score * scoreMultiplier * (odMultiplier - 1):N0}"));

            if (typingModWords.AddBonusSpaceHitObjects.Value)
                additionalMetrics.Add(new RulesetBeatmapAttribute.AdditionalMetric("Spaces Bonus Score", $"{bonusSpacesScore:N0}"));

            var attributes = new List<RulesetBeatmapAttribute>
            {
                // This will sadly result in displaying a large number in the mod selection, but it was positioned next to a smaller
                // attribute for this reason to not overlap. The resulting number will be close anyway because of the float formatting...
                new RulesetBeatmapAttribute("Total Score", @"TS", (float)max_score, (float)adjustedScore, (float)maxPossibleScore)
                {
                    Description = "Maximum achievable score based on selected mod customisation, which applies score adjustments. These values are affected by total mod score multiplier.",
                    AdditionalMetrics = additionalMetrics.ToArray()
                },
                new RulesetBeatmapAttribute("HP", @"HP", beatmapInfo.Difficulty.DrainRate, adjustedDifficulty.DrainRate, 10)
                {
                    Description = "Affects the harshness of health drain and the health penalties for missing."
                },
                new RulesetBeatmapAttribute("WPM", @"WPM", (float)wpm, (float)wpmAdjusted, (float)wpm)
                {
                    Description = "Approximate Words Per Minute based on beatmap's most common BPM. This only applies to the Words mod and ignores the extra spacing between words."
                },
                new RulesetBeatmapAttribute("OD", @"OD", beatmapInfo.Difficulty.OverallDifficulty, adjustedDifficulty.OverallDifficulty, 10)
                {
                    Description = "Affects total score and timing requirements for hits",
                    AdditionalMetrics = hitWindows.GetAllAvailableWindows()
                                                  .Reverse()
                                                  .Select(window => new RulesetBeatmapAttribute.AdditionalMetric(
                                                      $"{window.result.GetDescription().ToUpperInvariant()} hit window",
                                                      LocalisableString.Interpolate($@"±{hitWindows.WindowFor(window.result) / rate:0.##} ms"),
                                                      colours.ForHitResult(window.result)
                                                  ))
                                                  .ToArray()
                }
            };

            return attributes;
        }

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Regular.Keyboard };

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
    }
}
