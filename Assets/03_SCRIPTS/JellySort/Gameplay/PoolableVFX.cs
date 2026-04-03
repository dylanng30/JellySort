using Dylanng.Core;
using Dylanng.Core.Pooling;
using UnityEngine;

namespace JellySort.Gameplay
{
    public class PoolableVFX : PoolableObject
    {
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private string _poolKey;
        private float _duration;

        protected override void Awake()
        {
            base.Awake();
            if (_particle == null) _particle = GetComponent<ParticleSystem>();
            if (_particle != null) _duration = _particle.main.duration + _particle.main.startLifetime.constantMax;
        }

        public void Setup(string poolKey)
        {
            _poolKey = poolKey;
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            if (_particle != null)
            {
                _particle.Play();
            }
            Invoke(nameof(DespawnSelf), _duration);
        }

        private void DespawnSelf()
        {
            ServiceLocator.Get<PoolManager>().Despawn(_poolKey, this);
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            CancelInvoke(nameof(DespawnSelf));
            if (_particle != null)
            {
                _particle.Stop();
            }
        }
    }
}