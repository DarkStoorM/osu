// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Typing.Beatmaps;

namespace osu.Game.Rulesets.Typing.Mods
{
    // Note: This mod is not very good to play, because words are broken, and it's just a constant stream of characters separated by beats
    public class TypingModWords : TypingMod, IApplicableToBeatmap
    {
        public override string Name => "Word Letters";
        public override LocalisableString Description => "Converts all hit objects into letters from random words (preserves beatmap).";
        public override string Acronym => "WL";
        public override ModType Type => ModType.Conversion;

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            var typingBeatmap = (TypingBeatmap)beatmap;

            ModRNG = new Random(Seed.Value ??= RNG.Next());

            string wordStream = PrepareStream(typingBeatmap);
            using var charEnumerator = wordStream.GetEnumerator();

            foreach (var letter in typingBeatmap.HitObjects)
            {
                if (!charEnumerator.MoveNext())
                    throw new InvalidOperationException("Not enough characters in the stream to convert all hit objects into words!");

                letter.Letter = LetterToTypingAction(charEnumerator.Current);
            }
        }
    }
}
