using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Events.GameEvents
{
    public struct LevelStartedEvent : IEvent
    {
        public int LevelIndex;
    }
}
