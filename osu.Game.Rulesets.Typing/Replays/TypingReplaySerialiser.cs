// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Platform;
using osu.Game.Rulesets.Replays;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Typing.Replays
{
    public class TypingReplaySerialiser
    {
        private const string replay_storage_directory = $"rulesets/{TypingRuleset.SHORT_NAME}/replays";

        /// <summary>
        /// Header written to the replay file. Most likely unnecessary for custom rulesets, but maybe
        /// </summary>
        private const int replay_header = 0x76857687;

        /// <summary>
        /// Written after the header.
        /// <para/>This is only for completeness. May not really be necessary, because this assumes the replays may change in the future,
        /// but there is really nothing to change.
        /// </summary>
        private const byte replay_version = 1;

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

        /// <summary>
        /// Storage retrieved for <see cref="replay_storage_directory"/>.
        /// </summary>
        private readonly Storage replayStorage;

        public TypingReplaySerialiser(Storage storage)
        {
            replayStorage = storage.GetStorageForDirectory(replay_storage_directory);
        }

        /// <summary>
        /// Encodes and writes all replay frames to a file with the same id as the passed in score.
        /// </summary>
        public void WriteReplayFramesFromScore(Score score)
        {
            SetScoreHash(score);

            var replayFrames = score.Replay.Frames.OfType<TypingReplayFrame>().ToList();

            // Note: guard against maximum amount of frames when the replay gets ridiculously large, e.g. more than 3mb (?)
            if (replayFrames.Count == 0)
                return;

            // While this is not an issue on legacy rulesets, frames may be unordered on failed replays, which happens right
            // after the drain time. A couple frames are inserted in reversed time for some reason, and with a tiny
            // difference of 0.005-0.008~Ms. While those frames are insignificant, they should still be ordered anyway
            replayFrames = replayFrames.OrderBy(f => f.Time).ToList();

            using Stream stream = replayStorage.GetStream(createReplayFileName(score.ScoreInfo.ID), FileAccess.Write, FileMode.Create);
            using BinaryWriter binaryWriter = new BinaryWriter(stream);

            binaryWriter.Write(replay_header);
            binaryWriter.Write(replay_version);

            foreach (TypingReplayFrame replayFrame in replayFrames)
            {
                int maskedKeyPresses = 0;

                // Flip a specific bit for each TypingAction in the replay frame, resulting in writing a value such as
                // [...]0000 -> [...]0001 if only Q was pressed. It would become [...]0111 if Q/W/E was pressed at once
                // in a frame, assuming that is the order the letters are defined in the enum
                foreach (TypingAction keyPressed in replayFrame.Actions)
                    maskedKeyPresses |= 1 << (int)keyPressed;

                binaryWriter.Write(replayFrame.Time);
                binaryWriter.Write(maskedKeyPresses);
            }
        }

        /// <summary>
        /// Decodes the replay file that is associated with this score if it exists under the passed in score id.
        /// </summary>
        public void ReadAndAddReplayToScore(Score score)
        {
            // There is a replay already, no need to do decode it from the file again
            if (score.Replay.Frames.Count > 0)
                return;

            string replayFileName = createReplayFileName(score.ScoreInfo.ID);

            if (!replayStorage.Exists(replayFileName))
                return;

            using Stream stream = replayStorage.GetStream(replayFileName, FileAccess.Read, FileMode.Open);
            using BinaryReader binaryReader = new BinaryReader(stream);

            // The replay file was probably empty, which should not happen, but maybe it should be investigated instead
            // since there could be a serialisation issue while writing the replay to the file
            if (stream.Length < sizeof(int))
                return;

            int header = binaryReader.ReadInt32();
            int version = binaryReader.ReadByte();

            // In general, if the header is only used to guard against trying to open a replay from another ruleset,
            // it's most likely unnecessary since nobody cares what is happening in custom rulesets, so, this could
            // check could be removed.
            // Pretty much the same with the replay version, they probably don't even have to exist at all
            if (header != replay_header || version != replay_version)
                return;

            long replayLength = stream.Length - stream.Position;
            long frameCount = replayLength / frame_size;

            // The same situation as in the beginning of the file, but if header/version data was present, but no frames,
            // something must have gone completely wrong. Might as well limit the frame count, but should not be necessary
            if (frameCount == 0)
                return;

            List<ReplayFrame> replayFrames = new List<ReplayFrame>();

            for (int i = 0; i < frameCount; i++)
            {
                double time = binaryReader.ReadDouble();
                int maskedKeyPresses = binaryReader.ReadInt32();
                TypingReplayFrame replayFrame = new TypingReplayFrame(time);

                // The remaining bits from 32 are unused, because in reality, we only use letters + space, so only those will be read.
                // Keep converting the present bits to TypingAction so we get the correct key presses back as actions performed in that frame
                for (int bit = 0; bit < supported_key_count; bit++)
                {
                    if (((maskedKeyPresses >> bit) & 1) != 0)
                        replayFrame.Actions.Add((TypingAction)bit);
                }

                replayFrames.Add(replayFrame);
            }

            score.Replay.Frames = replayFrames;
            score.Replay.HasReceivedAllFrames = true;
        }

        public static void SetScoreHash(Score score) => score.ScoreInfo.Hash = $"{TypingRuleset.SHORT_NAME}-replay-{score.ScoreInfo.ID:N}";

        private static string createReplayFileName(Guid scoreId) => $"{scoreId:N}.osr";
    }
}
