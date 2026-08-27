// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Database;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Typing.Replays
{
    public class TypingReplayRealmStorage
    {
        private readonly RealmAccess realmAccess;
        private readonly TypingReplaySerialiser replaySerialiser;

        public TypingReplayRealmStorage(TypingReplaySerialiser serialiser, RealmAccess access)
        {
            replaySerialiser = serialiser;
            realmAccess = access;
        }

        public void StoreReplayOnScorePersisted(Score score)
        {
            TypingReplaySerialiser.SetScoreHash(score);

            realmAccess.RegisterForNotifications(
                realm => realm
                         .All<ScoreInfo>()
                         .Where(scoreInfo => scoreInfo.ID == score.ScoreInfo.ID && !scoreInfo.DeletePending),
                (scores, _) =>
                {
                    // Self note: this is very untested, not sure if this is the way (probably not).
                    // I don't know if multiple scores can appear in this notification
                    if (scores.Any())
                        replaySerialiser.WriteReplayFramesFromScore(score);
                });
        }
    }
}
