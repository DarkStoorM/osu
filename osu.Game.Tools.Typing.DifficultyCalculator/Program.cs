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

        private static readonly (string Name, Func<BeatLength, Mod> Factory)[] mods =
        {
            ("0K", createMod<TypingModEnglish0K>),
            ("1K", createMod<TypingModEnglish1K>),
            ("5K", createMod<TypingModEnglish5K>),
            ("10K", createMod<TypingModEnglish10K>),
            ("25K", createMod<TypingModEnglish25K>),
        };

        private static int Main()
        {
            Beatmap beatmap = createBeatmap(bpm, beatmap_drain_time_in_ms);

            DifficultyResult[] results = mods
                                         .Select(x => calculate(
                                             beatmap,
                                             x.Name,
                                             x.Factory(mod_beat_length)))
                                         .ToArray();

            Console.Clear();

            Console.WriteLine($"Beatmap: {beatmap.Metadata.Artist} - {beatmap.Metadata.Title} [{beatmap.BeatmapInfo.DifficultyName}]");
            Console.WriteLine();

            printTable(results);

            return 0;
        }

        // Note: all mod application is the same, so they will get the same data
        private static T createMod<T>(BeatLength beatLength) where T : TypingEnglishMod, new()
        {
            return new T
            {
                Seed = { Value = 1 },
                AdjustBeatLength = { Value = beatLength },
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

        private static DifficultyResult calculate(Beatmap beatmap, string name, Mod mod)
        {
            FlatWorkingBeatmap working = new FlatWorkingBeatmap(beatmap);
            TypingRuleset ruleset = new TypingRuleset();
            var calculator = ruleset.CreateDifficultyCalculator(working);
            List<Mod> beatmapMods = new List<Mod> { mod };

            if (include_double_time)
                beatmapMods.Add(new TypingModDoubleTime());

            TypingDifficultyAttributes? attributes = (TypingDifficultyAttributes)calculator.Calculate(beatmapMods);

            return new DifficultyResult(name, attributes);
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
                ("Finger Control", a => a.FingerControl),
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

        private sealed record DifficultyResult(string Name, TypingDifficultyAttributes Attributes);
    }
}
