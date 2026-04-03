using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Events.SystemEvents
{
    public struct SceneLoadedEvent : IEvent
    {
        public string SceneName;
    }
}