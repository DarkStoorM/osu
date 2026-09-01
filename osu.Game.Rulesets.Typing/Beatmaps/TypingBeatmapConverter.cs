// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Threading;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Beatmaps
{
    public class TypingBeatmapConverter : BeatmapConverter<TypingHitObject>
    {
        private readonly TypingAction[] allActions = Enum.GetValues<TypingAction>();

        /// <summary>
        /// Used for deterministic beatmap conversion. It's required to not break replays in the default no-mod gameplay.
        /// </summary>
        private readonly Random beatmapSeededRng;

        public TypingBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
            byte[] hash = System.Convert.FromHexString(Beatmap.BeatmapInfo.MD5Hash);
            int seed = 0;

            // "Combine" the bytes I guess? Probably doesn't do much anyway
            for (int i = 0; i < hash.Length; i += 4)
                seed ^= BitConverter.ToInt32(hash, i);

            beatmapSeededRng = new Random(seed);
        }

        public override bool CanConvert() => true;

        protected override IEnumerable<TypingHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            yield return createHitObject(original.Samples, original.StartTime);

            // For conversion of beatmaps with sliders/etc, it might be good to have an extra object for the tail.
            // There might be an issue with some beatmaps that have a very long slider/spinner at the end of the map,
            // but I'd consider that an edge case
            if (original is IHasDuration objectEnd)
                yield return createHitObject(original.Samples, objectEnd.EndTime);
        }

        protected override Beatmap<TypingHitObject> CreateBeatmap() => new TypingBeatmap();

        private TypingHitObject createHitObject(IList<HitSampleInfo> samples, double startTime)
        {
            return new TypingHitObject
            {
                Samples = samples,
                StartTime = startTime,
                Letter = allActions[beatmapSeededRng.Next(allActions.Length)],
            };
        }
    }
}
