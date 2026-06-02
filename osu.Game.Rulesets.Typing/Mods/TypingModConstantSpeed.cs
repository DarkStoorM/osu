// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Objects;
using osu.Game.Rulesets.Typing.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Typing.Mods
{
    public class TypingModConstantSpeed : Mod, IApplicableToDrawableRuleset<TypingHitObject>
    {
        public override string Name => "Constant Speed";
        public override string Acronym => "CS";
        public override LocalisableString Description => "No more tricky speed changes!";
        public override IconUsage? Icon => OsuIcon.ModConstantSpeed;
        public override ModType Type => ModType.Conversion;

        public void ApplyToDrawableRuleset(DrawableRuleset<TypingHitObject> drawableRuleset)
        {
            var typingRuleset = (DrawableTypingRuleset)drawableRuleset;
            typingRuleset.VisualisationMethod = ScrollVisualisationMethod.Constant;
        }
    }
}
