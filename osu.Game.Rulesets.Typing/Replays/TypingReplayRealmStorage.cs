// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

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

        // Delete-me-note: this should subscribe to the realm notifications for this particular score, and if it's persisted,
        // only then the replay should be serialised and stored for this score.
        public void StoreReplayOnScorePersisted(Score score) { }
    }
}
