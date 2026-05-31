// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly TypingAction[] allActionsWithoutSpace = Enum.GetValues<TypingAction>().Where(action => action != TypingAction.Space).ToArray();

        public TypingBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset) { }

        public override bool CanConvert() => true;

        protected override IEnumerable<TypingHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            yield return createHitObject(original.Samples, original.StartTime);

            // For conversion of beatmaps with sliders/etc, it might be good to have an extra object for the tail
            if (original is IHasDuration objectEnd)
                yield return createHitObject(original.Samples, objectEnd.EndTime);
        }

        protected override Beatmap<TypingHitObject> CreateBeatmap() => new TypingBeatmap();

        /// <summary>
        /// Returns a random key before the words provider is implemented, which will change how letters are picked.
        /// </summary>
        /// <param name="includeSpace">This will eventually be implemented to the callers, but for now ignore the fact that Space exists</param>
        private TypingAction randomTypingAction(bool includeSpace = false)
        {
            var values = includeSpace
                ? allActions
                : allActionsWithoutSpace;

            return values[Random.Shared.Next(values.Length)];
        }

        private TypingHitObject createHitObject(IList<HitSampleInfo> samples, double startTime)
        {
            return new TypingHitObject
            {
                Samples = samples,
                StartTime = startTime,
                Letter = randomTypingAction(),
            };
        }
    }
}
