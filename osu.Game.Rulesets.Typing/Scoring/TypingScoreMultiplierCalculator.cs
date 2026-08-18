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
            // HalfTime should greatly decrease the multiplier only for Halved beat length, because that customisation
            // option increases the mod multiplier. Both can't really coexist, because Halved beat length + HalfTime
            // adjustment will give free score when maximum value is applied, resulting in decreasing the speed that
            // matches Default beat length.
            Combination<TypingModHalfTime, TypingModWords>(hasMultiplier: (halfTime, words) => words.AdjustBeatLength.Value == BeatLengthAdjustment.Halved
                ? halfTime.SpeedChange.Value / 2
                : halfTime.SpeedChange.Value);

            // With DoubleTime, the HT issue is not that important since it would make no sense to apply DT + Doubled beat length
            // to play with the same speed. The resulting multiplier will still be 1 on 2x DT, so it will only make the song fast
            Single<TypingModDoubleTime>(hasMultiplier: doubleTime => doubleTime.SpeedChange.Value);

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
