// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Typing.Mods
{
    public class TypingModEnglish1K : TypingEnglishMod
    {
        public override string Name => "English 1k";
        public override string Acronym => "1K";
        public override LocalisableString Description => "English 1k dictionary";
        public override DictionarySize DictionarySize => DictionarySize.E1K;
    }
}
