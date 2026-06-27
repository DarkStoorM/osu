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
    public static class TypingHitEventExtensions
    {
        public static UnstableRateCalculationResult? CalculateKeyUnstableRate(
            this IReadOnlyList<HitEvent> hitEvents,
            TypingAction key,
            UnstableRateCalculationResult? result = null
        )
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

        public static bool AffectsUnstableRate(HitEvent e) =>
            AffectsUnstableRate(e.HitObject, e.Result);

        public static bool AffectsUnstableRate(HitObject hitObject, HitResult result) =>
            hitObject.HitWindows != HitWindows.Empty && result.IsHit();

        public class UnstableRateCalculationResult
        {
            public int LastProcessedIndex = -1;
            public int EventCount;
            public double SumOfSquares;
            public double Mean;

            public double Result =>
                EventCount == 0 ? 0 : 10.0 * Math.Sqrt(SumOfSquares / EventCount);
        }
    }
}
