// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Game.Utils;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Typing.Screens.Ranking.Statistics
{
    // Note: This is a direct copy of the color information from OsuColour for Star Rating, with some minor tweaks
    // as it didn't need to reflect the values 1:1
    public readonly record struct KeyCardData
    {
        private static readonly Colour4 default_gray = Color4Extensions.FromHex(@"333");

        public int HitEventsCount { get; }
        public int MissEventsCount { get; }
        public double UnstableRate { get; }

        public Colour4 BackgroundColour { get; }
        public Colour4 AdditionalInformationBackgroundColour { get; }
        public Colour4 TextColour { get; }
        public Colour4 BorderColour { get; }
        public Colour4 GlowColour { get; }

        // Note on the minimum UR: 80UR is an arbitrary number selected as an optimal value for "almost perfect" unstable rate
        private static readonly (float, Color4)[] unstable_rate_spectrum =
        {
            (10f, Color4Extensions.FromHex("aaaaaa")),
            (10f, Color4Extensions.FromHex("69d943")),
            (80f, Color4Extensions.FromHex("69d943")),
            (100f, Color4Extensions.FromHex("d9d450")),
            (120f, Color4Extensions.FromHex("d96e59")),
            (150f, Color4Extensions.FromHex("d9435f")),
            (180f, Color4Extensions.FromHex("ab3c9e")),
            (220f, Color4Extensions.FromHex("5755bd")),
            (260f, Color4Extensions.FromHex("18158e")),
            (310f, Color4.Black),
        };

        private static readonly (float, Color4)[] unstable_rate_text_spectrum =
        {
            (260f, Color4Extensions.FromHex("d9d450")),
            (290f, Color4Extensions.FromHex("d96e59")),
            (310f, Color4Extensions.FromHex("d9435f")),
            (330f, Color4Extensions.FromHex("ab3c9e")),
            (350f, Color4Extensions.FromHex("5755bd")),
        };

        public KeyCardData(int hitEventsCount, int missEventsCount, double unstableRate)
        {
            HitEventsCount = hitEventsCount;
            MissEventsCount = missEventsCount;
            UnstableRate = unstableRate;

            if (UnstableRate == 0)
            {
                BackgroundColour = default_gray;
                AdditionalInformationBackgroundColour = default_gray.Darken(0.75f);
                TextColour = default_gray.Lighten(0.5f);
                return;
            }

            BackgroundColour = colourForUnstableRate().Darken(0.25f);
            AdditionalInformationBackgroundColour = colourForUnstableRate().Darken(0.75f);
            TextColour = textColourForUnstableRate().Darken(0.25f);
            BorderColour = borderColourForUnstableRate();
            GlowColour = borderColourForUnstableRate().Opacity(0.1f);
        }

        private Colour4 colourForUnstableRate() => ColourUtils.SampleFromLinearGradient(unstable_rate_spectrum, (float)Math.Round(UnstableRate, 2, MidpointRounding.AwayFromZero));

        private Colour4 textColourForUnstableRate()
        {
            if (UnstableRate < 160)
                return Color4.Black.Opacity(0.75f);

            if (UnstableRate < 310)
                return Color4Extensions.FromHex(@"ffd966");

            return ColourUtils.SampleFromLinearGradient(unstable_rate_text_spectrum, (float)Math.Round(UnstableRate, 2, MidpointRounding.AwayFromZero));
        }

        private Colour4 borderColourForUnstableRate()
        {
            float ur = (float)Math.Round(UnstableRate, 2, MidpointRounding.AwayFromZero);
            Colour4 colour = ur >= 310
                ? ColourUtils.SampleFromLinearGradient(unstable_rate_text_spectrum, ur).Lighten(0.75f).Opacity(0.5f)
                : ColourUtils.SampleFromLinearGradient(unstable_rate_spectrum, ur).Lighten(0.75f);

            return colour;
        }
    }
}
