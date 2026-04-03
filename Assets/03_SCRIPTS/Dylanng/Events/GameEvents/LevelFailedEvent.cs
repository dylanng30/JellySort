using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Events.GameEvents
{
    public struct LevelFailedEvent : IEvent
    {
        public int LevelIndex;
    }
}
