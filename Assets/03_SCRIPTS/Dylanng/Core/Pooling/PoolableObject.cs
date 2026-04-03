
using Dylanng.Core.Base;

namespace Dylanng.Core.Pooling
{
    public abstract class PoolableObject : MonoBehaviourBase, IPoolable
    {
        public virtual void OnSpawn() => gameObject.SetActive(true);
        public virtual void OnDespawn() => gameObject.SetActive(false);
    }
}
