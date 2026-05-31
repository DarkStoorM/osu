// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
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
            // TODO: this is currently using a randomly taken letter, which was only made to have something on the scene
            // Change this later to get the next letter from some Word Provider that will be applied to this beatmap.
            // This may be overridden later through mods that regenerate the whole beatmap, so this will serve as a
            // non-destructive conversion if the player wants to preserve HitObjects, but it will split the words
            yield return new TypingHitObject
            {
                Samples = original.Samples,
                StartTime = original.StartTime,
                Letter = randomTypingAction(),
            };
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
    }
}
