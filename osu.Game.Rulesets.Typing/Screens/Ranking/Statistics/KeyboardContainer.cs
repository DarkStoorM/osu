// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Typing.Layouts;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osu.Game.Rulesets.Typing.Objects;
using osu.Game.Rulesets.Typing.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Typing.Screens.Ranking.Statistics
{
    public partial class KeyboardContainer : Container
    {
        public KeyboardContainer(ScoreInfo score)
        {
            AutoSizeAxes = Axes.Both;
            Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Children = new[]
                {
                    new KeyboardRowContainer(score, KeyboardRow.Top),
                    new KeyboardRowContainer(score, KeyboardRow.Home),
                    new KeyboardRowContainer(score, KeyboardRow.Bottom),
                }
            };
        }

        private static KeyboardKeyCard[] getKeyCards(ScoreInfo score, KeyboardRow row)
        {
            KeyboardLayout layout = new QwertyOrtholinearLayout();
            List<KeyboardKeyCard> keyCards = new List<KeyboardKeyCard>();

            var keys = layout.Keys
                             .Select(key => key.Value)
                             .Where(key => key.Row == row)
                             .ToList();

            foreach (PhysicalKey key in keys)
            {
                var keyHitEvents = score.HitEvents
                                        .Where(e => e.HitObject is TypingHitObject hitObject && hitObject.Letter == key.Character)
                                        .ToList();

                keyCards.Add(new KeyboardKeyCard(key.Character.ToString(),
                    keyHitEvents.Count,
                    keyHitEvents.CalculateKeyUnstableRate(key.Character)?.Result)
                );
            }

            return keyCards.ToArray();
        }

        private partial class KeyboardRowContainer : FillFlowContainer
        {
            public KeyboardRowContainer(ScoreInfo score, KeyboardRow row)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Horizontal;
                Children = getKeyCards(score, row);
                Margin = new MarginPadding { Left = 30 * (int)row };
            }
        }
    }
}
