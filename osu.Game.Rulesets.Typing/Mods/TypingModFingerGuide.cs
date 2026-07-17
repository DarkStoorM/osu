// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Typing.Objects.Drawables;

namespace osu.Game.Rulesets.Typing.Mods
{
    // TODO: Communicate this better, because this basically tells absolutely nothing to the user
    public class TypingModFingerGuide : Mod, IApplicableToDrawableHitObject
    {
        public override string Acronym => "FG";
        public override LocalisableString Description => "Colours objects as they would appear on touch-typing guides";
        public override string Name => "Finger Guide";
        public override ModType Type => ModType.DifficultyReduction; // Probably incorrect, but whatever

        public void ApplyToDrawableHitObject(DrawableHitObject drawable)
        {
            DrawableTypingHitObject drawableHit = (DrawableTypingHitObject)drawable;

            drawableHit.OverrideKeyColour = true;
        }
    }
}
