// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Replays
{
    public class TypingAutoGenerator : AutoGenerator<TypingReplayFrame>
    {
        public new Beatmap<TypingHitObject> Beatmap => (Beatmap<TypingHitObject>)base.Beatmap;

        public TypingAutoGenerator(IBeatmap beatmap)
            : base(beatmap) { }

        protected override void GenerateFrames()
        {
            Frames.Add(new TypingReplayFrame());

            foreach (TypingHitObject hitObject in Beatmap.HitObjects)
            {
                double time = hitObject.StartTime - 5;

                addFrame(time, hitObject.Letter);
            }
        }

        private void addFrame(double time, TypingAction direction)
        {
            Frames.Add(new TypingReplayFrame(direction) { Time = time });
            Frames.Add(new TypingReplayFrame { Time = time + KEY_UP_DELAY }); //Release the keys as well
        }
    }
}
