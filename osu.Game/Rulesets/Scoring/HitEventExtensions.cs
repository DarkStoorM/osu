// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Game.Audio;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Scoring
{
    public static class HitEventExtensions
    {
        /// <summary>
        /// Calculates the "unstable rate" for a sequence of <see cref="HitEvent"/>s.
        /// </summary>
        /// <remarks>
        /// Uses <a href="https://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Welford's_online_algorithm">Welford's online algorithm</a>.
        /// </remarks>
        /// <returns>
        /// A non-null <see langword="double"/> value if unstable rate could be calculated,
        /// and <see langword="null"/> if unstable rate cannot be calculated due to <paramref name="hitEvents"/> being empty.
        /// </returns>
        public static UnstableRateCalculationResult? CalculateUnstableRate(
            this IReadOnlyList<HitEvent> hitEvents,
            UnstableRateCalculationResult? result = null
        )
        {
            Debug.Assert(hitEvents.All(ev => ev.GameplayRate != null));

            result ??= new UnstableRateCalculationResult();

            // Handle rewinding in the simplest way possible.
            if (hitEvents.Count < result.EventCount + 1)
                result = new UnstableRateCalculationResult();

            // TODO: Split this into actual three sections, because it's wrongly calculated

            for (int i = result.EventCount; i < hitEvents.Count; i++)
            {
                HitEvent e = hitEvents[i];

                if (!AffectsUnstableRate(e))
                    continue;

                if (IsKat(e))
                {
                    result.KatEventCount++;

                    double currentKatValue = e.TimeOffset / e.GameplayRate!.Value;
                    double nextKatMean =
                        result.KatMean + (currentKatValue - result.KatMean) / result.KatEventCount;
                    result.KatSumOfSquares +=
                        (currentKatValue - result.KatMean) * (currentKatValue - nextKatMean);
                    result.KatMean = nextKatMean;
                }
                else
                {
                    result.DonEventCount++;

                    double currentDonValue = e.TimeOffset / e.GameplayRate!.Value;
                    double nextDonMean =
                        result.DonMean + (currentDonValue - result.DonMean) / result.DonEventCount;
                    result.DonSumOfSquares +=
                        (currentDonValue - result.DonMean) * (currentDonValue - nextDonMean);
                    result.DonMean = nextDonMean;
                }

                result.EventCount++;

                // Division by gameplay rate is to account for TimeOffset scaling with gameplay rate.
                // The global Unstable Rate will be calculated no matter what
                double currentValue = e.TimeOffset / e.GameplayRate!.Value;
                double nextMean = result.Mean + (currentValue - result.Mean) / result.EventCount;
                result.SumOfSquares += (currentValue - result.Mean) * (currentValue - nextMean);
                result.Mean = nextMean;
            }

            if (result.EventCount == 0)
                return null;

            return result;
        }

        public static bool IsKat(this HitEvent e) =>
            e.HitObject.Samples.Where(s =>
                s.Name == HitSampleInfo.HIT_CLAP || s.Name == HitSampleInfo.HIT_WHISTLE
            )
                .ToArray()
                .Length > 0;

        /// <summary>
        /// Calculates the average hit offset/error for a sequence of <see cref="HitEvent"/>s, where negative numbers mean the user hit too early on average.
        /// </summary>
        /// <returns>
        /// A non-null <see langword="double"/> value if unstable rate could be calculated,
        /// and <see langword="null"/> if unstable rate cannot be calculated due to <paramref name="hitEvents"/> being empty.
        /// </returns>
        public static double? CalculateAverageHitError(this IEnumerable<HitEvent> hitEvents)
        {
            double[] timeOffsets = hitEvents
                .Where(AffectsUnstableRate)
                .Select(ev => ev.TimeOffset)
                .ToArray();

            if (timeOffsets.Length == 0)
                return null;

            return timeOffsets.Average();
        }

        public static bool AffectsUnstableRate(HitEvent e) =>
            AffectsUnstableRate(e.HitObject, e.Result);

        public static bool AffectsUnstableRate(HitObject hitObject, HitResult result) =>
            hitObject.HitWindows != HitWindows.Empty && result.IsHit();

        /// <summary>
        /// Data type returned by <see cref="HitEventExtensions.CalculateUnstableRate"/> which allows efficient incremental processing.
        /// </summary>
        /// <remarks>
        /// This should be passed back into future <see cref="HitEventExtensions.CalculateUnstableRate"/> calls as a parameter.
        ///
        /// The optimisations used here rely on hit events being a consecutive sequence from a single gameplay session.
        /// When a new gameplay session is started, any existing results should be disposed.
        /// </remarks>
        public class UnstableRateCalculationResult
        {
            /// <summary>
            /// Total events processed. For internal incremental calculation use.
            /// </summary>
            public int EventCount;
            public int DonEventCount;
            public int KatEventCount;

            /// <summary>
            /// Last sum-of-squares value. For internal incremental calculation use.
            /// </summary>
            public double SumOfSquares;
            public double DonSumOfSquares;
            public double KatSumOfSquares;

            /// <summary>
            /// Last mean value. For internal incremental calculation use.
            /// </summary>
            public double Mean;
            public double DonMean;
            public double KatMean;

            /// <summary>
            /// The unstable rate.
            /// </summary>
            public double Result =>
                EventCount == 0 ? 0 : 10.0 * Math.Sqrt(SumOfSquares / EventCount);

            public double ResultForDons =>
                DonEventCount == 0 ? 0 : 10.0 * Math.Sqrt(DonSumOfSquares / DonEventCount);

            public double ResultForKats =>
                KatEventCount == 0 ? 0 : 10.0 * Math.Sqrt(KatSumOfSquares / KatEventCount);
        }
    }
}
