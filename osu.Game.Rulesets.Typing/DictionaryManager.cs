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
    public class DictionaryManager
    {
        private readonly Dictionary<DictionarySize, string[]> dictionaries = new Dictionary<DictionarySize, string[]>();

        public DictionaryManager(ResourceStore<byte[]> resources)
        {
            load(resources);
        }

        private void load(ResourceStore<byte[]> resources)
        {
            foreach (var size in Enum.GetValues<DictionarySize>())
            {
                string filename = getFilename(size);

                using var stream = resources.GetStream($"Resources/GPL/{filename}");

                if (stream == null)
                    throw new Exception($"Missing dictionary resource: {filename}");

                using var reader = new StreamReader(stream);

                string json = reader.ReadToEnd();

                string[] words = JsonSerializer.Deserialize<string[]>(json)
                                 ?? throw new Exception($"Failed to deserialize {filename}");

                dictionaries[size] = words;
            }
        }

        public string[] GetWords(DictionarySize size)
            => dictionaries[size];

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
