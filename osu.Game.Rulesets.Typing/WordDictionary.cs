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
        private const string resources_path = "Resources/GPL/";

        public static Dictionary<DictionarySize, string[]> CreateDictionaries(ResourceStore<byte[]> resources)
        {
            Dictionary<DictionarySize, string[]> wordDictionaries = new Dictionary<DictionarySize, string[]>();

            foreach (var dictionarySize in Enum.GetValues<DictionarySize>())
            {
                string filename = getFilename(dictionarySize);

                using var resourceStream = resources.GetStream($"{resources_path}{filename}");

                if (resourceStream == null)
                    throw new InvalidOperationException($"Failed to create the dictionary. The file {filename} was missing.");

                using var reader = new StreamReader(resourceStream);

                string json = reader.ReadToEnd();
                string[] words = JsonSerializer.Deserialize<string[]>(json) ?? throw new InvalidOperationException($"Failed to deserialize {filename}");

                wordDictionaries[dictionarySize] = words;
            }

            return wordDictionaries;
        }

        private static string getFilename(DictionarySize size) => size switch
        {
            DictionarySize.E0K => "E0K.json",
            DictionarySize.E1K => "E1K.json",
            DictionarySize.E5K => "E5K.json",
            DictionarySize.E10K => "E10K.json",
            DictionarySize.E25K => "E25K.json",
            DictionarySize.E450K => "E450K.json",
            _ => throw new ArgumentOutOfRangeException(nameof(size)),
        };
    }
}
