// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Typing.Layouts;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osu.Game.Rulesets.Typing.Mods;
using osu.Game.Rulesets.Typing.Objects;
using osu.Game.Rulesets.Typing.Scoring;

namespace osu.Game.Rulesets.Typing.Screens.Ranking.Statistics
{
    public partial class KeyTimingDistribution : Container
    {
        private readonly KeyboardLayout defaultKeyboardLayout = new QwertyStaggeredLayout();
        private readonly KeyDistributionContainer keyDistributionContainer;

        /// <summary>
        /// A hacky gradient of color with arbitrary unstable rate value range.
        /// </summary>
        private static ColorGradient? colorGradient { get; } = new ColorGradient(
            new ColorStop(0, new Colour4(0.00f, 0.95f, 0.25f, 1f)),
            new ColorStop(125, new Colour4(0.45f, 1.00f, 0.15f, 1f)),
            new ColorStop(175, new Colour4(1.00f, 0.95f, 0.10f, 1f)),
            new ColorStop(225, new Colour4(1.00f, 0.60f, 0.00f, 1f)),
            new ColorStop(275, new Colour4(1.00f, 0.18f, 0.18f, 1f)),
            new ColorStop(325, new Colour4(0.60f, 0.00f, 0.00f, 1f))
        );

        public KeyTimingDistribution(IReadOnlyList<HitEvent> hitEvents, IReadOnlyList<Mod> mods)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            TypingWordsMod? wordsMod = mods.OfType<TypingWordsMod>().FirstOrDefault();
            KeyboardLayout keyboardLayout = wordsMod == null
                ? defaultKeyboardLayout
                : wordsMod.SelectedKeyboardLayout;

            Child = keyDistributionContainer = new KeyDistributionContainer(hitEvents, keyboardLayout)
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
            };
        }

        /// <summary>
        /// Updates the key card based on judged hit with recalculated Unstable Rate for that hit.
        /// </summary>
        public void UpdateKeyCard(JudgementResult currentJudgementResult, IReadOnlyList<HitEvent> allHitEvents) => keyDistributionContainer.UpdateKeyCard(currentJudgementResult, allHitEvents);

        public partial class KeyDistributionContainer : FillFlowContainer
        {
            private readonly Dictionary<KeyboardRow, KeyboardRowContainer> keyboardRows = new Dictionary<KeyboardRow, KeyboardRowContainer>();

            public KeyDistributionContainer(IReadOnlyList<HitEvent> hitEvents, KeyboardLayout keyboardLayout)
            {
                Direction = FillDirection.Vertical;
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                keyboardRows.Add(KeyboardRow.Top, new KeyboardRowContainer(hitEvents, keyboardLayout, KeyboardRow.Top));
                keyboardRows.Add(KeyboardRow.Home, new KeyboardRowContainer(hitEvents, keyboardLayout, KeyboardRow.Home));
                keyboardRows.Add(KeyboardRow.Bottom, new KeyboardRowContainer(hitEvents, keyboardLayout, KeyboardRow.Bottom));

                Children = keyboardRows.Values.ToList();
            }

            public void UpdateKeyCard(JudgementResult judgementResult, IReadOnlyList<HitEvent> hitEvents)
            {
                TypingHitObject typingHitObject = (TypingHitObject)judgementResult.HitObject;

                keyboardRows[typingHitObject.CurrentKey.Row].UpdateKeyCard(typingHitObject, hitEvents);
            }
        }

        public partial class KeyboardRowContainer : FillFlowContainer
        {
            private readonly Dictionary<TypingAction, KeyboardKeyCard> keyCards = new Dictionary<TypingAction, KeyboardKeyCard>();

            public KeyboardRowContainer(IReadOnlyList<HitEvent> hitEvents, KeyboardLayout keyboardLayout, KeyboardRow row)
            {
                Direction = FillDirection.Horizontal;
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                Margin = new MarginPadding { Left = (int)row * 30 };

                createRowKeyCards(hitEvents, keyboardLayout, row);

                Children = keyCards.Values.ToList();
            }

            public void UpdateKeyCard(TypingHitObject typingHitObject, IReadOnlyList<HitEvent> hitEvents)
            {
                KeyCardData keyCardData = createCardData(hitEvents, typingHitObject.CurrentKey);

                keyCards[typingHitObject.CurrentKey.Character].UpdateKeyCard(keyCardData.HitEventsCount, keyCardData.UnstableRate, keyCardData.Colour);
            }

            private void createRowKeyCards(IReadOnlyList<HitEvent> hitEvents, KeyboardLayout keyboardLayout, KeyboardRow row)
            {
                var keys = keyboardLayout.Keys
                                         .Select(key => key.Value)
                                         .Where(key => key.Row == row)
                                         .ToList();

                foreach (PhysicalKey key in keys)
                {
                    KeyCardData keyCardData = createCardData(hitEvents, key);

                    keyCards.Add(key.Character, new KeyboardKeyCard(key.Character.ToString(),
                            keyCardData.HitEventsCount,
                            keyCardData.UnstableRate,
                            keyCardData.Colour
                        )
                    );
                }
            }
        }

        private static KeyCardData createCardData(IReadOnlyList<HitEvent> hitEvents, PhysicalKey key)
        {
            List<HitEvent> keyHitEvents = filterHitEventsByKey(hitEvents, key);
            double? unstableRate = keyHitEvents.CalculateKeyUnstableRate(key.Character)?.Result ?? 0;

            // Hit events per key should be at least ten to deem the unstable rate valid for this key, so we have to force zero
            // it out if there were not enough keypresses. The reason for this is that the unstable rate will converge at higher
            // amount of hits across the whole gameplay, so a very short beatmap should not yield valuable results
            if (keyHitEvents.Count < 10)
                unstableRate = null;

            Colour4? colour = colorGradient?.Evaluate(unstableRate);

            return new KeyCardData(keyHitEvents.Count, unstableRate, colour);
        }

        /// <summary>
        /// Returns only those hit events that affect the Unstable Rate.
        /// </summary>
        private static List<HitEvent> filterHitEventsByKey(IReadOnlyList<HitEvent> hitEvents, PhysicalKey key)
        {
            return hitEvents
                   .Where(e =>
                       ((TypingHitObject)e.HitObject).Letter == key.Character &&
                       TypingHitEventExtensions.AffectsUnstableRate(e))
                   .ToList();
        }

        private readonly record struct KeyCardData(int HitEventsCount, double? UnstableRate, Colour4? Colour);
    }
}
