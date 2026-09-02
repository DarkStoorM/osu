// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Objects;

namespace osu.Game.Rulesets.Typing.Scoring
{
    /// <summary>
    /// This is a copy of <see cref="HitEventExtensions"/>, with adjustments for <see cref="TypingAction"/>.
    /// </summary>
    public static class TypingHitEventExtensions
    {
        public static UnstableRateCalculationResult? CalculateKeyUnstableRate(this IReadOnlyList<HitEvent> hitEvents, TypingAction key, UnstableRateCalculationResult? result = null)
        {
            Debug.Assert(hitEvents.All(ev => ev.GameplayRate != null));

            result ??= new UnstableRateCalculationResult();

            if (hitEvents.Count < result.LastProcessedIndex + 1)
                result = new UnstableRateCalculationResult();

            for (int i = result.LastProcessedIndex + 1; i < hitEvents.Count; i++)
            {
                result.LastProcessedIndex = i;
                HitEvent e = hitEvents[i];

                if (key != ((TypingHitObject)e.HitObject).Letter)
                    continue;

                if (!AffectsUnstableRate(e))
                    continue;

                result.EventCount++;

                double currentValue = e.TimeOffset / e.GameplayRate!.Value;
                double nextMean = result.Mean + (currentValue - result.Mean) / result.EventCount;

                result.SumOfSquares += (currentValue - result.Mean) * (currentValue - nextMean);
                result.Mean = nextMean;
            }

            return result.EventCount == 0 ? null : result;
        }

        // Note: this is unused, because I still have to find a place for this... the keys on the key timing distribution
        // are pretty small. Either way, the hit error might not be necessary after all, since being far away from `0` just
        // means there was a beatmap timing or an offset issue
        public static double? CalculateAverageKeyHitError(this List<HitEvent> hitEvents, TypingAction typingAction)
        {
            int count = 0;
            double sum = 0;

            foreach (HitEvent hitEvent in hitEvents)
            {
                if (!AffectsUnstableRate(hitEvent) || ((TypingHitObject)hitEvent.HitObject).Letter != typingAction)
                    continue;

                sum += hitEvent.TimeOffset;
                count++;
            }

            return count == 0 ? null : sum / count;
        }

        public static bool AffectsUnstableRate(HitEvent e) => affectsUnstableRate(e.HitObject, e.Result);

        public class UnstableRateCalculationResult
        {
            public int LastProcessedIndex = -1;
            public int EventCount;
            public double SumOfSquares;
            public double Mean;

            public double Result => EventCount == 0 ? 0 : 10.0 * Math.Sqrt(SumOfSquares / EventCount);
        }

        private static bool affectsUnstableRate(HitObject hitObject, HitResult result) => hitObject.HitWindows != HitWindows.Empty && result.IsHit();
    }
}
