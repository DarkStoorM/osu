﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Screens.Ranking.Statistics
{
    /// <summary>
    /// Displays the unstable rate statistic for a given play.
    /// </summary>
    public partial class UnstableRateForDrumCentre : SimpleStatisticItem<double?>
    {
        /// <summary>
        /// Creates and computes an <see cref="UnstableRate"/> statistic specifically for Drum Centre (Dons).
        /// </summary>
        /// <param name="hitEvents">Sequence of <see cref="HitEvent"/>s to calculate the unstable rate based on.</param>
        public UnstableRateForDrumCentre(IReadOnlyList<HitEvent> hitEvents)
            : base("Don UR")
        {
            Value = hitEvents.CalculateUnstableRateForDrumCentre()?.Result;
        }

        protected override string DisplayValue(double? value) => value == null ? "(not available)" : value.Value.ToString(@"N2");
    }
}
