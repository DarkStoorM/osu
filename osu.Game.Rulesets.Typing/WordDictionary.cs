// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using osu.Framework.IO.Stores;
using osu.Game.Rulesets.Typing.Mods;

namespace osu.Game.Rulesets.Typing
{
    public static class WordDictionary
    {
        private const string resources_path = "Resources/Dictionaries/";

        public static Dictionary<DictionarySize, RankedWordGenerator> CreateDictionaries(ResourceStore<byte[]> resources)
        {
            var wordDictionaries = new Dictionary<DictionarySize, RankedWordGenerator>();

            foreach (var dictionarySize in Enum.GetValues<DictionarySize>())
            {
                using var resourceStream = resources.GetStream($"{resources_path}{dictionarySize}.json");

                if (resourceStream == null)
                    throw new InvalidOperationException($"Failed to create the dictionary. The resource file for {dictionarySize} dictionary was missing.");

                using var reader = new StreamReader(resourceStream);

                string json = reader.ReadToEnd();
                string[] words = JsonSerializer.Deserialize<string[]>(json) ?? throw new InvalidOperationException($"Failed to deserialize {dictionarySize} dictionary.");

                wordDictionaries[dictionarySize] = new RankedWordGenerator(words);
            }

            return wordDictionaries;
        }
    }
}
