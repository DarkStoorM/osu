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
            // HalfTime should reduce more than 0.01 per step in its 50-99% rate range. It will automatically scale
            // to whatever was the resulting multiplier from the mod itself.
            // It means that for every each letter spacing the factor will evaluate to 0.03, 0.02, 0.01 respectively,
            // assuming no other customisations were selected.
            // The reason for this change is that it was possible to select Narrow Letter Spacing + minimum
            // HalfTime rate to achieve the same typing speed with extra double score when there was no multiplier for HT
            Combination<TypingModHalfTime, TypingModWords>(hasMultiplier: (halfTime, modWords) =>
            {
                double wordsModMultiplier = getWordsModMultiplier(modWords);
                double halfTimeSubtractionFactor = wordsModMultiplier * 1.5;

                // Since HalfTime does not start from 1x, but 0.99, it should initially reduce the first 0.01 step.
                // Every 0.01 step of HalfTime, the score multiplier is reduced by scaled subtraction factor
                return wordsModMultiplier - halfTimeSubtractionFactor / 100 - (0.99 - halfTime.SpeedChange.Value) * halfTimeSubtractionFactor;
            });

            // HalfTime alone is not really used, because Words mod is required to actually play this ruleset,
            // but it's here just for completeness
            Single<TypingModHalfTime>(hasMultiplier: halfTime => halfTime.SpeedChange.Value);

            // DoubleTime doesn't require the same treatment as HalfTime, because the score difference between
            // Narrow and Wide Letter Spacing is effectively twice as big (1kk as opposed to 500k). Even if
            // Wide Letter Spacing is used with DoubleTime at 2x rate, the typing speed is identical to Default
            // Letter Spacing with 1x score multiplier, thus not requiring any changes
            Single<TypingModDoubleTime>(hasMultiplier: doubleTime => doubleTime.SpeedChange.Value);

            Single<TypingModWords>(hasMultiplier: getWordsModMultiplier);
        }

        private static double getWordsModMultiplier(TypingModWords modWords)
        {
            double multiplier = 1;

            // This is the major multiplier change, because this directly affects the amount of objects created by the mod.
            // This is still dodgy anyway, because the BPM dictates everything. If the beatmap was mapped to 240bpm for example,
            // it would require the usage of Default Letter Spacing, which gives the 1.0 multiplier. Then, given
            // how 120bpm beatmap would be identical with Narrow Letter Spacing and awarding twice the score is a bit confusing
            multiplier *= modWords.LetterSpacing.Value switch
            {
                LetterSpacing.Narrow => 2,
                LetterSpacing.Default => 1,
                LetterSpacing.Wide => 0.5,
                _ => 1
            };

            // The Downbeat Snap customisation creates a gap between the words so big that continuous typing is less straining
            // and this is reason why this value is so low. It also reduces the object count by around 60-65%, but that's not
            // directly relevant. What's important here is that normally, the recovery time between the words is exactly one letter,
            // and with this setting it can be maximum 9 letters long. Words too long to fit in a whole beat will extend the snap
            // to the next downbeat
            if (modWords.SnapWordsToDownbeat.Value)
                multiplier *= modWords.SnapWordsToDownbeat.Value ? 0.35 : 1;

            return multiplier;
        }
    }
}
