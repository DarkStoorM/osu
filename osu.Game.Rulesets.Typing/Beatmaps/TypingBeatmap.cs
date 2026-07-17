// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Beatmaps
{
    public class TypingBeatmap : Beatmap<TypingHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            // int hits = HitObjects.Count;

            return new[]
            {
                // Disabled until there is a way to get these stats on post-process (after mod application)
                // new BeatmapStatistic
                // {
                //     Name = "Characters",
                //     CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Circles),
                //     Content = hits.ToString(),
                //     BarDisplayLength = hits,
                // },
                // Note: rate change has no effect on this
                getWpmStatistic("HWPM", 2),
                getWpmStatistic("FBPM", 1),
                getWpmStatistic("DBPM", 0.5),
            };
        }

        private BeatmapStatistic getWpmStatistic(string label, double modBeatDivisor)
        {
            double beatLength = GetMostCommonBeatLength();

            // Standard typing WPM assumes 5 characters per word.
            // Main mod inserts letters every half beat, so:
            // - letterTime = beatLength / 2
            // - CPM = 120000 / beatLength
            // - WPM = CPM / 5 = 24000 / beatLength
            double wpm = 24000.0 / (beatLength / modBeatDivisor);

            return new BeatmapStatistic
            {
                Name = label,
                CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Circles),
                Content = $"~{wpm:N0}",
            };
        }
    }
}
