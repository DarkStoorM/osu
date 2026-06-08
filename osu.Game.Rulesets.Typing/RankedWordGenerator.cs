// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.Typing
{
    public sealed class RankedWordGenerator
    {
        private readonly string[] words;
        private readonly double[] cumulative;

        public RankedWordGenerator(IReadOnlyList<string> rankedWords, double s = 0.6)
        {
            words = rankedWords.ToArray();
            cumulative = new double[words.Length];

            double total = 0;

            for (int i = 0; i < words.Length; i++)
            {
                double rank = i + 1;
                double weight = 1.0 / Math.Pow(rank, s);

                total += weight;
                cumulative[i] = total;
            }

            for (int i = 0; i < cumulative.Length; i++)
                cumulative[i] /= total;
        }

        public string NextWord(Random random)
        {
            double roll = random.NextDouble();

            int index = Array.BinarySearch(cumulative, roll);

            if (index < 0)
                index = ~index;

            return words[Math.Clamp(index, 0, words.Length - 1)];
        }
    }
}
