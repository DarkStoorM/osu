// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Configuration.Tracking;
using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Typing.UI;

namespace osu.Game.Rulesets.Typing.Configuration
{
    public class TypingRulesetConfigManager : RulesetConfigManager<TypingRulesetSetting>
    {
        public TypingRulesetConfigManager(SettingsStore? settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            SetDefault(
                TypingRulesetSetting.ScrollTime,
                DrawableTypingRuleset.DEFAULT_SCROLL_TIME,
                DrawableTypingRuleset.MIN_SCROLL_TIME,
                DrawableTypingRuleset.MAX_SCROLL_TIME,
                DrawableTypingRuleset.SCROLL_TIME_STEP);

            SetDefault(TypingRulesetSetting.ScrollAdjustmentCount, 0.0);
        }

        public override TrackedSettings CreateTrackedSettings() => new TrackedSettings
        {
            new TrackedSetting<double>(TypingRulesetSetting.ScrollAdjustmentCount,
                val => new SettingDescription(
                    rawValue: val,
                    name: "Scroll Speed",
                    // Reverse the counter so it changes in the same direction as scroll speed (fast scroll = higher amount)
                    value: ((int)(DrawableTypingRuleset.MAX_SCROLL_ADJUSTMENT_AMOUNT - val)).ToString()
                )
            )
        };
    }

    public enum TypingRulesetSetting
    {
        ScrollTime,
        ScrollAdjustmentCount
    }
}
