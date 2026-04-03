using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Events.GameEvents
{
    public struct LevelCompletedEvent : IEvent
    {
        public int LevelIndex;
        public int Score;
    }
}
