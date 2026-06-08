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
            int hits = HitObjects.Count;

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = "Characters",
                    CreateIcon = () => new BeatmapStatisticIcon(BeatmapStatisticsIconType.Circles),
                    Content = hits.ToString(),
                    BarDisplayLength = hits,
                },
            };
        }
    }
}
