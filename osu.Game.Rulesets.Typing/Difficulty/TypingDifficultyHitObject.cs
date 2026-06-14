// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Rulesets.Difficulty.Preprocessing;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Typing.Layouts.KeyboardData;
using osuTK;

namespace osu.Game.Rulesets.Typing.Difficulty
{
    public class TypingDifficultyHitObject : DifficultyHitObject
    {
        public PhysicalKey PhysicalKey { get; }

        public HitObject? NextObject { get; }

        /// <summary>
        /// How many rows were switched between this object and previous.
        /// </summary>
        public double RowDelta { get; }

        public double DistanceFromPreviousKey { get; }

        public bool IsOnSameFinger { get; }

        public bool IsOnSameHand { get; }

        public bool IsOnSameRow { get; }

        public bool IsKeyRepeated { get; }

        public TypingDifficultyHitObject(HitObject current,
                                         HitObject previous,
                                         double clockRate,
                                         List<DifficultyHitObject> allObjects,
                                         int index,
                                         PhysicalKey currentKey,
                                         PhysicalKey previousKey,
                                         HitObject? nextObject)
            : base(current, previous, clockRate, allObjects, index)
        {
            PhysicalKey = currentKey;
            NextObject = nextObject;

            DistanceFromPreviousKey = Vector2.Distance(previousKey.Position, currentKey.Position);

            RowDelta = Math.Abs(previousKey.Row - currentKey.Row);
            IsOnSameFinger = previousKey.Hand == currentKey.Hand && previousKey.Finger == currentKey.Finger;
            IsOnSameHand = previousKey.Hand == currentKey.Hand;
            IsOnSameRow = previousKey.Row == currentKey.Row;
            IsKeyRepeated = previousKey.Character == currentKey.Character;
        }
    }
}
