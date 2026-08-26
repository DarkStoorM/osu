// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Platform;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Typing.Replays
{
    public class TypingReplaySerialiser
    {
        private const string replay_storage_directory = "ruleset-data/typing/replays";

        /// <summary>
        /// Header written to the replay file. Most likely unnecessary for custom rulesets, but maybe
        /// </summary>
        private const int replay_header = 0x76857687;

        /// <summary>
        /// Written after the header.
        /// <para/>This is only for completeness. May not really be necessary, because this assumes the replays may change in the future,
        /// but there is really nothing to change.
        /// </summary>
        private const byte version = 1;

        /// <summary>
        /// Frame Time and an integer, which is a mask containing <see cref="TypingAction"/> as bits flipped where action happened.
        /// </summary>
        private const byte frame_size = sizeof(double) + sizeof(int);

        /// <summary>
        /// Serves as an amount of bits to read.
        /// <para/>This will __most likely__ never change, because in this ruleset, the only keys that will ever be used are
        /// 26 keys for letters and Space.
        /// <para/>This could also be taken from enum value of <see cref="TypingAction"/>, but then, this would assume TypingAction will change.
        /// </summary>
        private const byte supported_key_count = 27;

        private Storage replayStorage;

        public TypingReplaySerialiser(Storage storage)
        {
            replayStorage = storage.GetStorageForDirectory(replay_storage_directory);
        }

        // Delete-me-note: encode the replay data for the key presses on a mask, where TypingAction flips a bit, e.g.
        // the mask changes from [...]0000 to [...]0001 if a key was pressed whose value in the enum was 1
        public void Write(Score score) { }

        // Delete-me-note: decode replay in the same manner, reading the bits from a mask and converting them into
        // TypingAction, then creating a relay frame with them.
        // Self note: I still don't know if it's necessary to add guards for e.g. max allowed replay frames, because some maps
        // are 20 minutes long, which is just an edge case anyway (gonna ignore the 1h one...)
        public void Read(Score score) { }

        public static string AddScoreHash(Score score) => score.ScoreInfo.Hash = $"typing-replay-{score.ScoreInfo.ID:N}";

        private static string createReplayFileName(Guid scoreId) => $"{scoreId:N}.otr";
    }
}
