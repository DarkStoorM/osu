// Copyright (c) ppy Pty Ltd <contact@ppy.sh>.Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Rulesets.Typing.Mods
{
    public abstract class TypingEnglishMod : Mod, IApplicableToBeatmap
    {
        protected abstract DictionarySize DictionarySize { get; }
        public override ModType Type => ModType.Conversion;
        public override string Acronym => Name;
        public override bool Ranked => false;

        public override Type[] IncompatibleMods => new[]
        {
            typeof(TypingModEnglish0K),
            typeof(TypingModEnglish1K),
            typeof(TypingModEnglish5K),
            typeof(TypingModEnglish10K),
            typeof(TypingModEnglish25K),
            typeof(TypingModEnglish450K),
        }.Except(new[] { GetType() }).ToArray();

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            Logger.Log($"Should apply {DictionarySize} to beatmap.");
        }
    }

    public enum DictionarySize
    {
        // For consistency, 0k was preferred over `EnglishSimple
        E0K,
        E1K,
        E5K,
        E10K,
        E25K,
        E450K,
    }
}
