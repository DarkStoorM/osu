// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;

namespace osu.Game.Rulesets.Typing
{
    public record ColorStop(float Value, Colour4 Color);

    public class ColorGradient
    {
        private readonly List<ColorStop> stops;

        public ColorGradient(params ColorStop[] stops)
        {
            this.stops = stops.OrderBy(s => s.Value).ToList();
        }

        public Colour4 Evaluate(double score)
        {
            float value = (float)score;

            if (value <= stops[0].Value)
                return stops[0].Color;

            for (int i = 0; i < stops.Count - 1; i++)
            {
                var a = stops[i];
                var b = stops[i + 1];

                if (value <= b.Value)
                {
                    float t = (value - a.Value) / (b.Value - a.Value);
                    return lerp(a.Color, b.Color, smoothStep(t));
                }
            }

            return stops[^1].Color;
        }

        private static Colour4 lerp(Colour4 a, Colour4 b, float t) => new Colour4(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, 1f);

        private static float smoothStep(float t) => t * t * (3f - 2f * t);
    }
}
