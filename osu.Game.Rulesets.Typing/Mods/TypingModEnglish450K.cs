// Copyright (c) ppy Pty Ltd <contact@ppy.sh>.Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Typing.Mods
{
    public class TypingModEnglish450K : TypingEnglishMod
    {
        public override string Name => "English 450k";
        public override string Acronym => "450K";
        public override LocalisableString Description => "English 450k dictionary";
        protected override DictionarySize DictionarySize => DictionarySize.E450K;
    }
}
