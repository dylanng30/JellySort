using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Core.Base
{
    public abstract class ManagerBase : MonoBehaviourBase, IManager
    {
        public abstract void Initialize();
    }
}