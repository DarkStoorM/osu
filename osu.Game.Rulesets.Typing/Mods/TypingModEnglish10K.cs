// Copyright (c) ppy Pty Ltd <contact@ppy.sh>.Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Typing.Mods
{
    public class TypingModEnglish10K : TypingEnglishMod
    {
        public override string Name => "English 10k";
        public override string Acronym => "10K";
        public override LocalisableString Description => "English 10k dictionary";
        protected override DictionarySize DictionarySize => DictionarySize.E10K;
    }
}
