// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Taiko.Beatmaps;
using osu.Game.Rulesets.Taiko.Objects;

namespace osu.Game.Rulesets.Taiko.Mods
{
    /// <summary>
    /// The reason why this exists: https://gist.github.com/DarkStoorM/060db882956e249bb029a71e471f73c4
    /// </summary>
    public class TaikoModFullRandom : Mod, IApplicableToBeatmap
    {
        public override string Name => "Full Random";
        public override string Acronym => "FR";
        public override LocalisableString Description => "Goodbye beatmap, hello chaos!";
        public override ModType Type => ModType.Fun;
        public override double ScoreMultiplier => 0.8;

        public override string ExtendedIconInformation
        {
            get
            {
                string label = InsertOneSixthTriplet.Value ? "1/6" : string.Empty;

                // Note: Only show the "++" if both options are enabled to indicate that we are playing on harder
                // version of 1/6
                if (InsertOneSixthTriplet.Value && LongerOneSixth.Value)
                    label += "++";

                // This isn't really needed, but when 1/6 is off, we will display the kat-to-don ratio, with the don
                // being on the left side. Kind of confusing, but it's just a visual representation of the slider. This
                // should only be allowed when we actually change the ratio
                if (!InsertOneSixthTriplet.Value && KatToDonRatio.Value != 0.5f)
                {
                    int katRatio = (int)(KatToDonRatio.Value * 100);

                    label += $"{100 - katRatio}:{katRatio}";
                }

                return label;
            }
        }

        public override Type[] IncompatibleMods =>
            [.. base.IncompatibleMods, typeof(TaikoModRandom), typeof(TaikoModSwap)];
        public override IconUsage? Icon => OsuIcon.ModRandom;

        [SettingSource("Longest Pattern Length")]
        public Bindable<PatternLength> LongestPatternLength { get; } =
            new Bindable<PatternLength>(PatternLength.Seven);

        [SettingSource("Colour Seed", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> ColourSeed { get; } = new Bindable<int?>();

        [SettingSource("Pattern Length Seed", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> PatternLengthSeed { get; } = new Bindable<int?>();

        [SettingSource(
            "Maximum Count Of Consecutive Monocolours",
            SettingControlType = typeof(SettingsNumberBox)
        )]
        public Bindable<int?> MaximumCountOfConsecutiveMonocolours { get; } = new Bindable<int?>();

        [SettingSource(
            "Turn Kiai Into Streams (EXPERIMENTAL)",
            "Incompatible with Timing Control Point spam!",
            SettingControlType = typeof(SettingsCheckbox)
        )]
        public BindableBool TurnKiaiIntoStreams { get; } = new BindableBool();

        [SettingSource("Generate At Double BPM", SettingControlType = typeof(SettingsCheckbox))]
        public BindableBool GenerateAtDoubleBPM { get; } = new BindableBool();

        [SettingSource("Insert 1/6", SettingControlType = typeof(SettingsCheckbox))]
        public BindableBool InsertOneSixthTriplet { get; } = new BindableBool();

        [SettingSource("1/6 Insertion Chance", SettingControlType = typeof(SettingsSlider<float>))]
        public BindableFloat OneSixthInsertionChance { get; } =
            new BindableFloat(0.05f)
            {
                MinValue = 0.0f,
                MaxValue = 0.5f,
                Precision = 0.01f,
            };

        [SettingSource("1/6 Insertion Seed", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> InsertionSeed { get; } = new Bindable<int?>();

        [SettingSource("1/6 Colour Seed", SettingControlType = typeof(SettingsNumberBox))]
        public Bindable<int?> OneSixthColourSeed { get; } = new Bindable<int?>();

        [SettingSource(
            "Invert Starting Rhythm Change Colour",
            SettingControlType = typeof(SettingsCheckbox)
        )]
        public BindableBool InvertColourOnRhythmChangeStart { get; } = new BindableBool(true);

        [SettingSource(
            "Invert Colour After Rhythm Change",
            SettingControlType = typeof(SettingsCheckbox)
        )]
        public BindableBool InvertColourAfterRhythmChange { get; } = new BindableBool(true);

        [SettingSource("Longer 1/6", SettingControlType = typeof(SettingsCheckbox))]
        public BindableBool LongerOneSixth { get; } = new BindableBool();

        [SettingSource(
            "Kat to Don Ratio",
            "Will not work well with monocolour restriction",
            SettingControlType = typeof(SettingsSlider<float>)
        )]
        public BindableFloat KatToDonRatio { get; } =
            new BindableFloat(0.5f)
            {
                MinValue = 0.0f,
                MaxValue = 1.0f,
                Precision = 0.001f,
            };

        /// <summary>
        /// List of "faulty" hit objects to remove from the beatmap after the generation is done, captured at timing
        /// changes.
        /// </summary>
        private readonly List<Hit?> faultyHitObjectsToRemove = new List<Hit?>();

        /// <summary>
        /// List of Kiai times (pairs of start-end) used by the <c>Turn Kiai Into Streams</c>, which inserts a 1/4
        /// stream
        /// </summary>
        private readonly List<KiaiTime> kiaiTimes = [];

        private TaikoBeatmap taikoBeatmap = null!;

        /// <summary>
        /// Helper reference which will eventually be stored for removal at generation time. This reference will be
        /// captured when a Timing Control Point is reached that changes the BPM - object too close to the new timing is
        /// removed.
        /// </summary>
        private Hit? lastHitObjectCreated;

        /// <summary>
        /// The current Timing Control Point used for comparison with eventual, new Timing Control Point.
        /// </summary>
        private TimingControlPoint currentTimingControlPoint = null!;

        private TimingControlPoint lastUsedTimingControlPoint = null!;

        private Random colourRNG = null!;
        private Random patternLengthRNG = null!;
        private Random insertionChanceRNG = null!;
        private Random oneSixthRNG = null!;

        /// <summary>
        /// Used to advance the time in the beatmap.
        /// </summary>
        private double currentTime;

        private double startGenerationAt;
        private double endGenerationAt;

        /// <summary>
        /// Base beat division for the current timing point (1/1).
        /// </summary>
        private double beatOne;

        private double beatOneHalf => beatOne / 2;
        private double beatOneFourth => beatOne / 4;
        private double beatOneSixth => beatOne / 6;

        /// <summary>
        /// Checks if we have actual two hit objects that we can use for the bounds. We only care about Hits, and not
        /// drumrolls or swells.
        /// </summary>
        private bool hasAtLeastTwoHitObjects =>
            taikoBeatmap.HitObjects.Select(h => h is Hit).Count() >= 2;

        /// <summary>
        /// Will interrupt any object insertion.
        /// </summary>
        private bool isStillWithinPlayingBounds => currentTime <= endGenerationAt;

        /// <summary>
        /// True if we are currently inside a "valid" Kiai section. See <see cref="initialiseKiaiTimes"/>.
        /// </summary>
        private bool isInKiaiTime =>
            kiaiTimes.Any(k => currentTime >= k.StartTime && currentTime <= k.EndTime);

        private bool hasTimingPointChanged =>
            !currentTimingControlPoint.Equals(lastUsedTimingControlPoint);

        /// <summary>
        /// Used by 1/6 pattern generation to prevent the insertion in case there is a Timing Control Point change in
        /// the next two beats
        /// </summary>
        private bool willTimingPointChangeSoon =>
            !currentTimingControlPoint.Equals(
                taikoBeatmap.ControlPointInfo.TimingPointAt(currentTime + beatOne * 2)
            );

        private TimingControlPoint timingPointAtCurrentTime =>
            taikoBeatmap.ControlPointInfo.TimingPointAt(currentTime);

        public TaikoModFullRandom()
        {
            MaximumCountOfConsecutiveMonocolours.ValueChanged += _ =>
            {
                if (MaximumCountOfConsecutiveMonocolours.Value == 0)
                    MaximumCountOfConsecutiveMonocolours.Value = 1;
            };

            KatToDonRatio.ValueChanged += change =>
                WeightedRandom.AdjustHitObjectRatio(change.NewValue);
        }

        public void ApplyToBeatmap(IBeatmap beatmap)
        {
            taikoBeatmap = (TaikoBeatmap)beatmap;

            // We need at least two object to even apply this mod. Should probably throw or something 🤔
            if (!hasAtLeastTwoHitObjects)
                return;

            initialiseSettings();

            if (TurnKiaiIntoStreams.Value)
                initialiseKiaiTimes();

            // Start fresh, because we will be generating a completely new difficulty
            taikoBeatmap.HitObjects.Clear();

            // Last hit object that will be used in eventual colour inversion when 1/6 starts (Hit before triplet)
            Hit? lastHitObjectInRegularPattern = null;

            // Last hit object that was generated in a 1/6 pattern, used for colour inversion of the next hit object
            // outside the 1/6 pattern - this will only affect a regular Hit inserted in a pattern
            Hit? lastHitObjectInOneSixthPattern = null;
            bool wasOneSixthGeneratedRecently = false;
            bool generateInfinitely = LongestPatternLength.Value == PatternLength.Unlimited;

            // Just treat Unlimited as a single hit object, it will later be spaced by 1/4 anyway if this was selected
            int longestPatternLength = generateInfinitely ? 1 : (int)LongestPatternLength.Value;

            List<Hit> currentPattern = [];

            // Reset every time we end a pattern or interrupt it with a 1/6
            int monocolourCountInPattern = 1;
            HitType? currentHitType = null;

            while (isStillWithinPlayingBounds)
            {
                currentPattern.Clear();

                int patternLength = patternLengthRNG.Next(1, longestPatternLength);

                // The pattern length override takes two steps (only for the Turn Kiai Into Streams option), because we
                // have to calculate the length of the Kiai section in hit objects (1/4) once we enter the Kiai, and the
                // step two of this is to ignore the rolled pattern length before this check just in case we happen to
                // have a longer pattern that could potentially overlap with the Kiai start before we even execute the
                // check
                if (TurnKiaiIntoStreams.Value)
                    patternLength = overrideLongestPatternLengthIfInKiai(patternLength);

                // We could collect the enum values instead and roll to ensure odd values, but, meh
                if (patternLength % 2 == 0)
                    patternLength++;

                for (int i = 0; i < patternLength; i++)
                {
                    // The step two
                    if (TurnKiaiIntoStreams.Value && isTooCloseToKiai())
                    {
                        if (i % 2 == 0)
                            patternLength = 1;
                    }

                    Hit? hitObject = createRandomHitObject();

                    if (hitObject == null)
                        break;

                    // If the max monocolour number was set, the colour will be inverted on reaching the limit
                    if (
                        hitObject.Type == currentHitType
                        && MaximumCountOfConsecutiveMonocolours.Value != null
                    )
                    {
                        monocolourCountInPattern++;

                        if (monocolourCountInPattern > MaximumCountOfConsecutiveMonocolours.Value)
                        {
                            monocolourCountInPattern = 1;

                            invertHitObjectColour(hitObject);
                        }
                    }

                    // Stored for the comparison in the next iteration
                    currentHitType = hitObject.Type;

                    // We always immediately add the first hit object, because it basically already **is** a pattern
                    currentPattern.Add(hitObject);

                    // If there happens to be a BPM change, we will just stop the pattern generation here and allow it
                    // to be spaced by 1/2, because on some maps this will create an almost stacked/doubled object,
                    // disrupting the rhythm. This will still be broken anyway
                    if (hasTimingPointChanged)
                        break;

                    // If we are almost at the start/end of the Kiai, don't allow generating 1/6
                    if (!isTooLateForOneSixthPattern())
                    {
                        // If the 1/6 was recently generated, we will not allow to generate it again for this hit object,
                        // because the flow makes more sense if there is at least one 1/4 hit object between the 1/6s
                        if (wasOneSixthGeneratedRecently)
                        {
                            wasOneSixthGeneratedRecently = false;

                            if (InvertColourAfterRhythmChange.Value)
                                invertHitObjectColour(hitObject, lastHitObjectInOneSixthPattern);
                        }
                        else if (
                            InsertOneSixthTriplet.Value
                            && !wasOneSixthGeneratedRecently
                            && !willTimingPointChangeSoon
                            && insertionChanceRNG.NextDouble() < OneSixthInsertionChance.Value
                        )
                        {
                            monocolourCountInPattern = 1;
                            wasOneSixthGeneratedRecently = true;

                            // This is 1/6-specific, because we want to force a different colour on rhythm change
                            if (InvertColourOnRhythmChangeStart.Value)
                                invertHitObjectColour(hitObject, lastHitObjectInRegularPattern);

                            // Each consecutive hit object in this triplet will be spaced by a 1/6 beat
                            lastHitObjectInOneSixthPattern = addOneSixthTriplet(currentPattern);

                            // Appended 1/6 triplets are basically a replacement for one 1/4, so we have to skip the
                            // extra two hit objects that were added. The third hit object lands on 1/4 anyway, so we
                            // can't skip it (+3), otherwise we would shorten the calculated stream length during Kiai
                            if (TurnKiaiIntoStreams.Value && isInKiaiTime)
                                i += 2;

                            // Extend the initial 1/6 by 3 extra hit objects, but less frequently
                            if (
                                LongerOneSixth.Value
                                && insertionChanceRNG.NextDouble()
                                    < OneSixthInsertionChance.Value / 2
                            )
                            {
                                lastHitObjectInOneSixthPattern = addOneSixthTriplet(currentPattern);

                                if (TurnKiaiIntoStreams.Value && isInKiaiTime)
                                    i += 2;
                            }
                        }
                    }
                    else
                    {
                        // Zzz... another workaround for the Kiai option. We didn't get to call the colour inversion if
                        // we were too close. We don't really have to do this, but the colour inversion options exist
                        // for a reason after all
                        if (
                            TurnKiaiIntoStreams.Value
                            && wasOneSixthGeneratedRecently
                            && InvertColourOnRhythmChangeStart.Value
                            && InvertColourAfterRhythmChange.Value
                        )
                        {
                            invertHitObjectColour(hitObject, lastHitObjectInOneSixthPattern);
                        }
                    }

                    // Always space everything within a pattern by a 1/4 beat (except the last object)
                    if (i < patternLength - 1)
                        advanceTime(beatOneFourth);

                    // Stored for eventual colour inversion by 1/6 patterns
                    lastHitObjectInRegularPattern = hitObject;
                }

                taikoBeatmap.HitObjects.AddRange(currentPattern);

                // All patterns are separated by a 1/2 beat, even if it's just a single note. The only exception is when
                // we want to generate just a single stream, then we don't space them with usual 1/2 beat
                double spacingBeat = generateInfinitely ? beatOneFourth : beatOneHalf;

                advanceTime(spacingBeat);
            }

            // Clear anything that was marked as garbage hits caused by Timing Control Points -workaround, doesn't fix
            // everything :/
            if (faultyHitObjectsToRemove.Count > 0)
                taikoBeatmap.HitObjects.RemoveAll(h => faultyHitObjectsToRemove.Contains(h));

            cleanThingsUp();
        }

        private void initialiseSettings()
        {
            colourRNG = new Random(ColourSeed.Value ??= RNG.Next());
            patternLengthRNG = new Random(PatternLengthSeed.Value ??= RNG.Next());
            insertionChanceRNG = new Random(InsertionSeed.Value ??= RNG.Next());
            oneSixthRNG = new Random(OneSixthColourSeed.Value ??= RNG.Next());

            WeightedRandom.AdjustHitObjectRatio(KatToDonRatio.Value);

            // We only need the StartTime of the first/last objects, because we have to insert hit objects within the
            // actual playable bounds of this beatmap that are defined by Hits, and not drumrolls or swells. In some
            // beatmaps this might look weird if someone decides to put a drumroll/swell after the actual perceived
            // start of the beatmap
            startGenerationAt = taikoBeatmap.HitObjects.First(h => h is not IHasDuration).StartTime;
            endGenerationAt = taikoBeatmap.HitObjects.Last(h => h is not IHasDuration).StartTime;

            // Move the current time to the start time of the first object - we will start generating from there
            currentTime = startGenerationAt;

            // Set the actual BPM where the first object is in case there are multiple Timing Control Points for stuff
            // like drumrolls, so we don't use BPMs like 9999 from ninja objects
            currentTimingControlPoint = timingPointAtCurrentTime;

            updateBPMAndBeatLengths(currentTimingControlPoint);
        }

        /// <summary>
        /// Initialises all the "usable" Kiai times that we will be used for the pattern-to-stream conversion. This is
        /// only for the <c>Turn Kiai Into Streams</c> option and is just a workaround for the "misplaced" Effect
        /// Control Points, which will incorrectly offset the hit objects if we just use the regular Kiai start/end
        /// times, because they are not always snapped, so the streams could potentially start in a wrong spot: on a
        /// Blue divisor, which makes the beatmap unplayable due to everything being completely off-beat.
        /// </summary>
        private void initialiseKiaiTimes()
        {
            IReadOnlyList<EffectControlPoint> effectControlPoints = taikoBeatmap
                .ControlPointInfo
                .EffectPoints;
            List<EffectControlPoint> kiaiSections = effectControlPoints
                .Where(e => e.KiaiMode)
                .ToList();

            // Kiai needs to be at least 5 full beats (-margin of error) or 3 seconds. It wouldn't make sense to
            // consider something a stream if it was shorter than that. This is also used to check if the Kiai sections
            // are too close
            double kiaiThreshold = Math.Max(3000, beatOne * 5 - beatOneFourth);

            kiaiSections.ForEach(kiaiStartingPoint =>
            {
                // Snap the Kiai start time (and later the end time) to the closest 1/1 beat divisor in case it was
                // placed off-beat, which is often done to control when the SV change happens, people like doing that.
                // Without it, it could potentially offset the next pattern by 1/4 in a wrong spot
                double kiaiSnappedStartTime = taikoBeatmap.ControlPointInfo.GetClosestSnappedTime(
                    kiaiStartingPoint.Time,
                    1
                );
                EffectControlPoint? kiaiEndPoint = effectControlPoints.FirstOrDefault(
                    effectControlPoint =>
                        effectControlPoint.Time > kiaiStartingPoint.Time
                        && !effectControlPoint.KiaiMode
                );

                // There might be a situation where the Kiai just never stops (?), so we will use the earlier defined
                // end of the beatmap in that case
                if (kiaiEndPoint == null)
                {
                    kiaiTimes.Add(new KiaiTime(kiaiSnappedStartTime, endGenerationAt));

                    return;
                }

                double snappedKiaiEndTime = taikoBeatmap.ControlPointInfo.GetClosestSnappedTime(
                    kiaiEndPoint.Time,
                    1
                );

                // Only use this start-end pair if the length of the kiai section is above threshold
                if (snappedKiaiEndTime - kiaiStartingPoint.Time >= kiaiThreshold)
                    kiaiTimes.Add(new KiaiTime(kiaiSnappedStartTime, snappedKiaiEndTime));
            });
        }

        /// <summary>
        /// If the <c>Turn Kiai Into Streams</c> option is enabled, this will override the pattern length to the length
        /// of the Kiai section that the current time is in. The calculated pattern length takes only the 1/4 into
        /// account as we will be generating a constant stream of hit objects. Note: probably due to time inaccuracies,
        /// this can return an even number of notes, so it's adjusted later.
        /// </summary>
        private int overrideLongestPatternLengthIfInKiai(int patternLength)
        {
            if (!isInKiaiTime)
                return patternLength;

            // Find the Kiai section that we are currently in, if any
            KiaiTime? kiaiSection = getCurrentKiaiTimeOrNull();

            if (kiaiSection == null)
                return patternLength;

            // The extra subtraction is used to eventually move the start forward in case the previously generated
            // pattern was too long and overlapped with the Kiai Start
            return (int)(
                (
                    kiaiSection.EndTime
                    - kiaiSection.StartTime
                    - (currentTime - kiaiSection.StartTime)
                ) / beatOneFourth
            );
        }

        /// <summary>
        /// Checks if we are too close to both Start and End of the Kiai, which requires at least one beat. This is only
        /// specific to the <c>Turn Kiai Into Streams</c> option, so if this option is <c>off</c>, this will always
        /// return <c>false</c>. This only affects 1/6.
        /// </summary>
        /// <remarks>
        /// This is required to not interrupt the 1/6 right before or right at the end of the Kiai, which could
        /// potentially disrupt the rhythm if we override the pattern length to the length of the Kiai section.
        /// </remarks>
        private bool isTooLateForOneSixthPattern()
        {
            if (!TurnKiaiIntoStreams.Value)
                return false;

            KiaiTime? nextKiaiStart = getNextClosestKiaiOrNull();

            // We found a Kiai section in front of us, check if we are too close to it
            if (!isInKiaiTime && nextKiaiStart != null)
                return isTooCloseToKiaiPoint(nextKiaiStart.StartTime);

            // If there were no more __next__ Kiai sections, check if we were inside one instead
            KiaiTime? kiaiSection = getCurrentKiaiTimeOrNull();

            if (kiaiSection == null)
                return false;

            return isInKiaiTime && isTooCloseToKiaiPoint(kiaiSection.EndTime);
        }

        /// <summary>
        /// Checks if the current time is too close to the next Kiai start, if any.
        /// </summary>
        private bool isTooCloseToKiai()
        {
            KiaiTime? closeKiai = getNextClosestKiaiOrNull();

            if (closeKiai == null)
                return false;

            return !isInKiaiTime && closeKiai.StartTime - currentTime <= beatOne;
        }

        /// <summary>
        /// Checks if the current time is too close to the known Kiai point.
        /// </summary>
        private bool isTooCloseToKiaiPoint(double referenceKiaiTime)
        {
            return currentTime <= referenceKiaiTime && (referenceKiaiTime - currentTime <= beatOne);
        }

        /// <summary>
        /// Returns the Kiai section that the current time is in, if any.
        /// </summary>
        private KiaiTime? getCurrentKiaiTimeOrNull()
        {
            return kiaiTimes.FirstOrDefault(
                kiaiTime => currentTime >= kiaiTime.StartTime && currentTime <= kiaiTime.EndTime
            );
        }

        /// <summary>
        /// Returns the Kiai section in front of <see cref="currentTime"/>, if any.
        /// </summary>
        private KiaiTime? getNextClosestKiaiOrNull()
        {
            return kiaiTimes.FirstOrDefault(
                kiaiTime => currentTime < kiaiTime.StartTime && currentTime < kiaiTime.EndTime
            );
        }

        /// <summary>
        /// Should be called before advancing the <see cref="currentTime"/> to make sure the hit object is placed
        /// correctly.
        /// </summary>
        private void updateBPMAndBeatLengths(TimingControlPoint timingPoint)
        {
            double beatLength = timingPoint.BeatLength;

            if (GenerateAtDoubleBPM.Value)
                beatLength /= 2;

            beatOne = beatLength;
        }

        private void advanceTime(double beat) => currentTime += beat;

        private void invertHitObjectColour(Hit hitObjectToInvert)
        {
            hitObjectToInvert.Type =
                hitObjectToInvert.Type == HitType.Centre ? HitType.Rim : HitType.Centre;
        }

        /// <summary>
        /// Inverts the colour of this hit object based on the previous hit object. Only applied if they are the same.
        /// </summary>
        private void invertHitObjectColour(Hit hitObjectToInvert, Hit? previousHitObject)
        {
            if (previousHitObject == null)
                return;

            // Invert the colour only if the previous hit object is of the same type
            if (hitObjectToInvert.Type != previousHitObject.Type)
                return;

            invertHitObjectColour(hitObjectToInvert);
        }

        /// <summary>
        /// Creates a TaikoHitObject at given time with a random type (colour)
        /// </summary>
        /// <param name="overrideRNG">RNG for the colour generation. Omitting this will use <see cref="colourRNG"/>.</param>
        /// <returns><c>null</c> if the end of the map was reached.</returns>
        private Hit? createRandomHitObject(Random? overrideRNG = null)
        {
            // Yeah
            lastUsedTimingControlPoint = (TimingControlPoint)currentTimingControlPoint.DeepClone();

            // Just don't do anything if we reached the end of the map inside a longer pattern. This will force stop the
            // further generation
            if (!isStillWithinPlayingBounds)
                return null;

            Random rng = overrideRNG ?? colourRNG;

            // We have to check if the Timing Control Point has changed at the current time before the hit object is
            // created, because the BPM and current time will need to be adjusted so the hit objects are placed
            // correctly. Note: some beatmaps have very off-beat Timing Control Points, so I don't care if like 0.001%
            // of them are broken 🤷‍♂️ the following block is just an attempt to fix most of those beatmaps that have
            // more Timing Control Points so we have a bit more viable beatmaps to play
            if (!currentTimingControlPoint.Equals(timingPointAtCurrentTime))
            {
                currentTimingControlPoint = timingPointAtCurrentTime;
                currentTime = currentTimingControlPoint.Time;

                // If the difference in time between the last inserted object (if any) and the current time taken from
                // the Timing Control Point is smaller than the smallest beat length we are using (1/6), just remove the
                // previous note, because the timing changes can cause almost a stacked insertion, which is basically
                // unreadable and incorrect anyway. This is just a workaround to the weird timing changes the artist
                // decided to use in their songs, BUT, there are still cases where this can happen, so I don't care
                // about those maps. It can also create a small gap, but it's better than stacked hits
                if (
                    lastHitObjectCreated != null
                    && currentTime - lastHitObjectCreated.StartTime < beatOneSixth
                )
                    faultyHitObjectsToRemove.Add(lastHitObjectCreated);

                updateBPMAndBeatLengths(currentTimingControlPoint);
            }

            Hit hitObject = new Hit
            {
                StartTime = currentTime,
                Type = WeightedRandom.GetRandomWeightedColour(rng),
            };

            // The game crashes without this, I don't know if you are supposed to do this, though
            hitObject.ApplyDefaults(taikoBeatmap.ControlPointInfo, taikoBeatmap.Difficulty);

            lastHitObjectCreated = hitObject;

            return hitObject;
        }

        /// <returns>Last hit object from the triplet, used later to eventually invert colours.</returns>
        private Hit? addOneSixthTriplet(List<Hit> currentPattern)
        {
            Hit? hitObject = null;

            for (int i = 0; i < 3; i++)
            {
                // Triplets have to be pushed by a 1/6 beat
                advanceTime(beatOneSixth);

                hitObject = createRandomHitObject(oneSixthRNG);

                // Edge case where the very last note rolls a 1/6
                if (hitObject == null)
                    break;

                currentPattern.Add(hitObject);
            }

            return hitObject;
        }

        public enum PatternLength
        {
            One = 1,
            Three = 3,
            Five = 5,
            Seven = 7,
            Nine = 9,
            Eleven = 11,
            Unlimited = 99,
        }

        private class KiaiTime
        {
            public double StartTime { get; }
            public double EndTime { get; }

            public KiaiTime(double start, double end)
            {
                StartTime = start;
                EndTime = end;
            }
        }

        /// <summary>
        /// Cleans up all the used resources after the generation is done, because it leaves quite a lot of persistent
        /// garbage that can spike up to 100MB of memory per mod application.
        /// </summary>
        private void cleanThingsUp()
        {
            faultyHitObjectsToRemove.Clear();
            kiaiTimes.Clear();
            taikoBeatmap = null!;
            colourRNG = null!;
            patternLengthRNG = null!;
            insertionChanceRNG = null!;
            oneSixthRNG = null!;
            lastHitObjectCreated = null;
            currentTimingControlPoint = null!;
            lastUsedTimingControlPoint = null!;
        }

        /// <summary>
        /// Weighted random for colour generation.
        /// </summary>
        private static class WeightedRandom
        {
            private static float donWeight;
            private static float katWeight;

            /// <summary>
            /// Adjusts the weights of the hit object colours based on the new value of the colour ratio slider.
            /// </summary>
            /// <param name="ratio">Ratio, where don's value is the left side of the colour ratio slider.</param>
            public static void AdjustHitObjectRatio(float ratio)
            {
                // The Kat weight is always defined by the value of the colour ratio slider, then the rest of the ratio
                // is reserved for the Don weight
                katWeight = ratio;
                donWeight = 1 - katWeight;
            }

            /// <summary>
            /// Returns a random hit object colour based on the weights. The weights are adjusted by the colour ratio
            /// slider. See: <see cref="KatToDonRatio"/>.
            /// </summary>
            /// <param name="rng">Which rng should be used for the colour generation.</param>
            public static HitType GetRandomWeightedColour(Random rng)
            {
                float[] items = [donWeight, katWeight];
                float totalWeight = items.Sum();
                float threshold = rng.NextSingle() * totalWeight;

                for (int i = 0; i < items.Length; i++)
                {
                    // Note: [0] is always Don, [1] is always Kat, so if the first item meets the threshold, just return
                    // Don
                    if (threshold < items[i])
                        return i == 0 ? HitType.Centre : HitType.Rim;

                    threshold -= items[i];
                }

                throw new InvalidOperationException("This should never happen (?)");
            }
        }
    }
}
