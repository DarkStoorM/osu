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
        /// <para/>DO NOT CHANGE __after__ release as this will affect existing replays. Tweak this once and leave it alone.
        /// </summary>
        private readonly Dictionary<int, int> wordWeightsByLength = new Dictionary<int, int>
        {
            { 1, 20 },
            { 3, 35 },
            { 5, 45 },
            { 7, 25 },
            { 9, 15 },
            { 11, 8 },
            { 13, 4 },
            { 15, 2 },
            { 17, 2 },
            { 19, 2 },
        };

        /// <summary>
        /// Defines only those word weights that were present during the dictionary processing.
        /// <para/>Used for weighted random access and word generation.
        /// </summary>
        private readonly Dictionary<int, int> availableWordWeights = new Dictionary<int, int>();

        private readonly Dictionary<int, List<string>> wordListsByLength = new Dictionary<int, List<string>>();

        private readonly int totalWeight;

        public WeightedRandomWordGenerator(IReadOnlyList<string> words)
        {
            // The sort is not really necessary here since only the length is picked at weighted random and individual words are
            // uniformly distributed
            string[] sortedWords = words.OrderBy(word => word.Length)
                                        .ThenBy(word => word)
                                        .ToArray();

            // Prepare the secondary dictionary by separating the provided list into word groups by length
            // so that we don't have to iterate through the entire dictionary again
            for (int i = 0; i < sortedWords.Length; i++)
            {
                string word = words[i];

                // We could deliberately drop words with even length, but that would be a false assumption that dictionary is valid
                Debug.Assert(word.Length % 2 != 0);

                if (wordListsByLength.TryGetValue(word.Length, out List<string>? wordsList))
                    wordsList.Add(word);
                else
                    wordListsByLength.Add(word.Length, new List<string> { word });
            }

            // Only store the weights that are actually used by the processed dictionary
            foreach ((int key, _) in wordListsByLength)
                availableWordWeights.Add(key, wordWeightsByLength[key]);

            totalWeight = availableWordWeights.Values.Sum();
        }

        public string NextWord(Random random)
        {
            int wordLength = getWeightedWordLength(random);
            var wordsList = wordListsByLength[wordLength];

            return wordsList[random.Next(wordsList.Count)];
        }

        private int getWeightedWordLength(Random random)
        {
            int threshold = random.Next(totalWeight);

            foreach ((int length, int weight) in availableWordWeights)
            {
                if (threshold < weight)
                    return length;

                threshold -= weight;
            }

            throw new InvalidOperationException("Failed to select a weighted word length.");
        }
    }
}
