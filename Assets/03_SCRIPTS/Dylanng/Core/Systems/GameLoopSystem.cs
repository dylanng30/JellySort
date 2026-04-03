using Dylanng.Core.Base;

namespace Dylanng.Core.Systems
{
    public class GameLoopSystem : SystemBase
    {
        public override void Initialize()
        {
            ServiceLocator.Register<GameLoopSystem>(this);
            // Setup core update loop logic, tick dependencies, etc.
        }
    }
}
