// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Typing.Scoring;

namespace osu.Game.Rulesets.Typing.Mods
{
    public interface ITypingRateAdjustmentMod : IApplicableToHitObject, IApplicableToBeatmap
    {
        BindableNumber<double> SpeedChange { get; }

        // Note: This is applied to the actual objects that are already present in the beatmap (without Words mod application)
        void IApplicableToHitObject.ApplyToHitObject(HitObject hitObject)
        {
            if (hitObject.HitWindows is TypingHitWindows typingHitWindows)
                typingHitWindows.SpeedMultiplier = SpeedChange.Value;
        }

        // Note 2: since Words mods removes all objects and creates NEW ones, the speed change from previously applied DT
        // customisation was basically deleted and the hit windows were not being changed, so we have to re-apply this
        // change to newly created letters by the mod
        void IApplicableToBeatmap.ApplyToBeatmap(IBeatmap beatmap)
        {
            foreach (var hitObject in beatmap.HitObjects)
            {
                if (hitObject.HitWindows is TypingHitWindows typingHitWindows)
                    typingHitWindows.SpeedMultiplier = SpeedChange.Value;
            }
        }
    }
}
