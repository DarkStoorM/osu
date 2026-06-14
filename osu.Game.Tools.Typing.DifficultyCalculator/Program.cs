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

        private static readonly (string Name, Func<BeatLength, int, Mod> Factory)[] mods =
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
                                         .Select(x => calculateAveraged(
                                             beatmap,
                                             x.Name,
                                             seed => x.Factory(mod_beat_length, seed)))
                                         .ToArray();

            Console.Clear();

            Console.WriteLine($"Beatmap: {beatmap.Metadata.Artist} - {beatmap.Metadata.Title} [{beatmap.BeatmapInfo.DifficultyName}]");
            Console.WriteLine();

            printTable(results);

            return 0;
        }

        private static T createMod<T>(BeatLength beatLength, int seed) where T : TypingEnglishMod, new()
        {
            return new T
            {
                Seed = { Value = seed },
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

        private static DifficultyResult calculateAveraged(
            Beatmap beatmap,
            string name,
            Func<int, Mod> modFactory)
        {
            double star = 0;
            double fingerControl = 0;
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
                        name,
                        modFactory(seed))
                    .Attributes;

                last = attributes;

                star += attributes.StarRating;
                fingerControl += attributes.FingerControl;
                keyTravel += attributes.KeyTravel;
                retrigger += attributes.Retrigger;
                rowSwitch += attributes.RowSwitch;
                speed += attributes.Speed;
                typingFatigue += attributes.TypingFatigue;
                wordLength += attributes.WordLength;
            }

            return new DifficultyResult(
                name,
                new TypingDifficultyAttributes
                {
                    StarRating = star / seed_count,
                    FingerControl = fingerControl / seed_count,
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
