// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Mods;

namespace osu.Game.Rulesets.Typing.Scoring
{
    public class TypingScoreMultiplierCalculator : ScoreMultiplierCalculator
    {
        public TypingScoreMultiplierCalculator(ScoreMultiplierContext context)
            : base(context)
        {
            // This is still dodgy anyway, because the BPM dictates everything. If the beatmap was mapped to 240bpm for example,
            // it would require the usage of default, defined full beat length, which gives the 1.0 multiplier. Then, given
            // how 120bpm beatmap would be identical with halved beat length and awarding twice the score is a bit confusing.
            // In the end, I guess it's probably the same with HalfTime on high bpm and DoubleTime on low bpm
            Single<TypingModWords>(hasMultiplier: words => words.AdjustBeatLength.Value switch
            {
                // So, the reason for doing it this way is that the mod may generate twice as many objects, because the
                // default beat length calculated from the beatmap is doubled
                BeatLengthAdjustment.Halved => 2,
                BeatLengthAdjustment.Default => 1,
                BeatLengthAdjustment.Doubled => 0.5,
                _ => 1
            });
        }
    }
}
