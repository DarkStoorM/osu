// Copyright (c) ppy Pty Ltd <contact@ppy.sh>.Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Typing.Mods
{
    public class TypingModEnglish0K : TypingEnglishMod
    {
        public override string Name => "English 0k";
        public override string Acronym => "0K";
        public override LocalisableString Description => @"Simple English dictionary";
        protected override DictionarySize DictionarySize => DictionarySize.E0K;
    }
}
