// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Typing;
using osu.Game.Rulesets.Typing.Difficulty;
using osu.Game.Rulesets.Typing.Mods;

namespace osu.Game.Tools.Typing.DifficultyCalculator
{
    internal class Program
    {
        private const bool include_double_time = false;
        private const double bpm = 140;
        private const double beatmap_drain_time_in_ms = 180000;
        private const BeatLength mod_beat_length = BeatLength.Half;

        private const int seed_count = 100;

        private static readonly (DictionarySize size, Func<BeatLength, int, DictionarySize, Mod> Factory)[] mods =
        {
            (DictionarySize.Curated, createMod),
            (DictionarySize.E0K, createMod),
            (DictionarySize.E1K, createMod),
            (DictionarySize.E5K, createMod),
        };

        private static int Main()
        {
            Beatmap beatmap = createBeatmap(bpm, beatmap_drain_time_in_ms);

            DifficultyResult[] results = mods
                                         .Select(x => calculateAveraged(
                                             beatmap,
                                             x.size,
                                             seed => x.Factory(mod_beat_length, seed, x.size)))
                                         .ToArray();

            Console.Clear();

            Console.WriteLine($"Beatmap: {beatmap.Metadata.Artist} - {beatmap.Metadata.Title} [{beatmap.BeatmapInfo.DifficultyName}]");
            Console.WriteLine();

            printTable(results);

            return 0;
        }

        private static TypingModWords createMod(BeatLength beatLength, int seed, DictionarySize size)
        {
            return new TypingModWords
            {
                Seed = { Value = seed },
                AdjustBeatLength = { Value = beatLength },
                DictionarySize = { Value = size }
            };
        }

        private static Beatmap createBeatmap(double bpm, double timeInMs)
        {
            Beatmap beatmap = new Beatmap
            {
                HitObjects = new List<HitObject>
                {
                    new HitObject { StartTime = 0 },
                    new HitObject { StartTime = timeInMs }
                }
            };

            beatmap.ControlPointInfo.Add(0, new TimingControlPoint { BeatLength = 60000 / bpm });

            return beatmap;
        }

        private static DifficultyResult calculateAveraged(
            Beatmap beatmap,
            DictionarySize size,
            Func<int, Mod> modFactory)
        {
            double star = 0;
            double keyTravel = 0;
            double retrigger = 0;
            double rowSwitch = 0;
            double speed = 0;
            double typingFatigue = 0;
            double wordLength = 0;

            TypingDifficultyAttributes? last = null;

            for (int seed = 1; seed <= seed_count; seed++)
            {
                var attributes = calculate(
                        beatmap,
                        size,
                        modFactory(seed))
                    .Attributes;

                last = attributes;

                star += attributes.StarRating;
                keyTravel += attributes.KeyTravel;
                retrigger += attributes.Retrigger;
                rowSwitch += attributes.RowSwitch;
                speed += attributes.Speed;
                typingFatigue += attributes.TypingFatigue;
                wordLength += attributes.WordLength;
            }

            return new DifficultyResult(
                size,
                new TypingDifficultyAttributes
                {
                    StarRating = star / seed_count,
                    KeyTravel = keyTravel / seed_count,
                    Retrigger = retrigger / seed_count,
                    RowSwitch = rowSwitch / seed_count,
                    Speed = speed / seed_count,
                    TypingFatigue = typingFatigue / seed_count,
                    WordLength = wordLength / seed_count,

                    // preserve structural fields from last run (or recompute if needed)
                    MaxCombo = last!.MaxCombo,
                });
        }

        private static DifficultyResult calculate(Beatmap beatmap, DictionarySize size, Mod mod)
        {
            FlatWorkingBeatmap working = new FlatWorkingBeatmap(beatmap);
            TypingRuleset ruleset = new TypingRuleset();
            var calculator = ruleset.CreateDifficultyCalculator(working);
            List<Mod> beatmapMods = new List<Mod> { mod };

            if (include_double_time)
                beatmapMods.Add(new TypingModDoubleTime());

            TypingDifficultyAttributes? attributes = (TypingDifficultyAttributes)calculator.Calculate(beatmapMods);

            return new DifficultyResult(size, attributes);
        }

        private static void printTable(IReadOnlyList<DifficultyResult> results)
        {
            const int row_width = 22;
            const int column_width = 12;

            Console.Write($"{"Attribute",-row_width}");

            foreach (DifficultyResult result in results)
                Console.Write($"{result.Name,column_width}");

            Console.WriteLine();
            Console.WriteLine(new string('-', row_width + column_width * results.Count));

            var rows = new (string Name, Func<TypingDifficultyAttributes, double> Selector)[]
            {
                ("Star Rating", a => a.StarRating),
                ("Hit Objects", a => a.MaxCombo),
                ("Key Travel", a => a.KeyTravel),
                ("Retrigger", a => a.Retrigger),
                ("Row Switch", a => a.RowSwitch),
                ("Speed", a => a.Speed),
                ("Typing Fatigue", a => a.TypingFatigue),
                ("Word Length", a => a.WordLength),
            };

            foreach ((string name, Func<TypingDifficultyAttributes, double> selector) in rows)
            {
                Console.Write($"{name,-row_width}");

                foreach (DifficultyResult result in results)
                    Console.Write($"{selector(result.Attributes),column_width:F4}");

                Console.WriteLine();
            }
        }

        private sealed record DifficultyResult(DictionarySize Name, TypingDifficultyAttributes Attributes);
    }
}
