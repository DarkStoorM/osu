// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace osu.Game.Rulesets.Typing
{
    public sealed class WeightedRandomWordGenerator
    {
        /// <summary>
        /// Defines arbitrarily chosen weights for each word length.
        /// <para/>Those numbers were chosen to roughly say: "How many times certain length is chosen per 100 rolls".
        /// <para/>DO NOT CHANGE __after__ release as this will affect existing replays.
        /// </summary>
        private readonly Dictionary<int, int> wordWeightsByLength = new Dictionary<int, int>
        {
            { 1, 25 },
            { 3, 25 },
            { 5, 35 },
            { 7, 20 },
            { 9, 13 },
            { 11, 7 },
            { 13, 4 },
            { 15, 2 },
            { 17, 1 },
            { 19, 1 },
        };

        private readonly Dictionary<int, List<string>> wordsListByLength = new Dictionary<int, List<string>>();

        private readonly int totalWeight;

        public WeightedRandomWordGenerator(IReadOnlyList<string> words)
        {
            // Prepare the secondary dictionary by separating the provided list into word groups by length
            // so that we don't have to iterate through the entire dictionary again
            for (int i = 0; i < words.Count; i++)
            {
                string word = words[i];

                // We could deliberately drop words with even length, but that would be a false assumption that dictionary is valid
                Debug.Assert(word.Length % 2 != 0);

                if (wordsListByLength.TryGetValue(word.Length, out List<string>? wordsList))
                    wordsList.Add(word);
                else
                    wordsListByLength.Add(word.Length, new List<string> { word });
            }

            totalWeight = wordWeightsByLength.Values.Sum();
        }

        public string NextWord(Random random)
        {
            int wordLength = getWeightedWordLength(random);
            var wordsList = wordsListByLength[wordLength];

            return wordsList[random.Next(wordsList.Count)];
        }

        private int getWeightedWordLength(Random random)
        {
            int threshold = random.Next(totalWeight);

            foreach ((int length, int weight) in wordWeightsByLength)
            {
                if (threshold < weight)
                    return length;

                threshold -= weight;
            }

            throw new InvalidOperationException("Failed to select a weighted word length.");
        }
    }
}
