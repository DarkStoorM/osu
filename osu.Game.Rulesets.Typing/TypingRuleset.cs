// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.IO.Stores;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Beatmaps;
using osu.Game.Rulesets.Typing.Difficulty;
using osu.Game.Rulesets.Typing.Layouts;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Rulesets.Typing.Scoring;
using osu.Game.Rulesets.Typing.Screens.Ranking.Statistics;
using osu.Game.Rulesets.Typing.UI;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Statistics;

namespace osu.Game.Rulesets.Typing
{
    public class TypingRuleset : Ruleset
    {
        public override string Description => "osu!typing";
        public override string ShortName => "typing";

        public IKeyboardLayout KeyboardLayout => new QwertyOrtholinearLayout();

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod>? mods = null) => new DrawableTypingRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new TypingBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) => new TypingDifficultyCalculator(RulesetInfo, beatmap);

        public override HealthProcessor CreateHealthProcessor(double drainStartTime) => new TypingHealthProcessor();

        public static Dictionary<DictionarySize, RankedWordGenerator> RankedDictionaries { get; private set; } = new Dictionary<DictionarySize, RankedWordGenerator>();

        public TypingRuleset()
        {
            // Note: ruleset seems to be instantiated every time a beatmapset is selected, so the dictionaries should only be created once
            if (RankedDictionaries.Count != 0)
                return;

            var resources = new ResourceStore<byte[]>(new DllResourceStore(typeof(TypingRuleset).Assembly));

            RankedDictionaries = WordDictionary.CreateDictionaries(resources);
        }

        public override IResourceStore<byte[]> CreateResourceStore() => new DllResourceStore(typeof(TypingRuleset).Assembly);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            // Note: for this ruleset, the only extra mods I decided to migrate are: HT, DT, DA, CS (auto is self-explanatory).
            // The reason for this was to have a fine control through rate change, and better control of the
            // Accuracy and Scroll Speed. No other mods are necessary. CS replaces HR/EZ anyway
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
                        new MultiMod(
                            new TypingModEnglish0K(),
                            new TypingModEnglish1K(),
                            new TypingModEnglish5K()
                        )
                    };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override StatisticItem[] CreateStatisticsForScore(ScoreInfo score, IBeatmap playableBeatmap)
        {
            return new[]
            {
                new StatisticItem("Key timing distribution", () => new KeyboardContainer(score), requiresHitEvents: false)
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

        public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Regular.Keyboard };

        // Leave this line intact. It will bake the correct version into the ruleset on each build/release.
        public override string RulesetAPIVersionSupported => CURRENT_RULESET_API_VERSION;
    }
}
