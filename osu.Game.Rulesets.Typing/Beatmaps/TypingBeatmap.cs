// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Beatmaps
{
    public class TypingBeatmap : Beatmap<TypingHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            // Note: the reason why this is empty is that statistics are not affected by mods, at least for now,
            // and since the main mod if affecting the beatmap in a way that it regenerates the difficulty completely,
            // inserting new objects, this is not reflected in the statistics. My assumption here is that ApplyToBeatmap
            // changes a copy of the original that is used during the gameplay, thus not having the changes visible, and
            // the statistics will only display the original beatmap's data.
            // In the end, this is intended, because the whole purpose of the gameplay is to regenerate the beatmap with
            // new "patterns", sort of like playing with Mania Random, not having a fixed, single seed.
            // A solution here would be to write a beatmap converter for this, which I don't see as worth doing.
            // This is also the main reason why there can't be a display of things like "Words Count".
            return Array.Empty<BeatmapStatistic>();
        }
    }
}
