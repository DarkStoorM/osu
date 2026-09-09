// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Typing.Scoring;

namespace osu.Game.Rulesets.Typing.Mods
{
    // Note: this uses the osu!mania rate change application
    public interface ITypingRateAdjustmentMod : IApplicableToHitObject
    {
        BindableNumber<double> SpeedChange { get; }

        void IApplicableToHitObject.ApplyToHitObject(HitObject hitObject)
            => ((TypingHitWindows)hitObject.HitWindows).SpeedMultiplier = SpeedChange.Value;
    }
}
