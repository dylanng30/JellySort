using System;
using UnityEngine;
using Dylanng.Core.Base;
using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Core.Systems
{
    public class UpdateSystem : SystemBase
    {
        private event Action<float> OnTickEvent;
        private event Action<float> OnLateTickEvent;

        public override void Initialize()
        {
            ServiceLocator.Register<UpdateSystem>(this);
        }

        public void Register(ITickable tickable) => OnTickEvent += tickable.Tick;
        public void Unregister(ITickable tickable) => OnTickEvent -= tickable.Tick;
        
        public void RegisterLate(ILateTickable tickable) => OnLateTickEvent += tickable.LateTick;
        public void UnregisterLate(ILateTickable tickable) => OnLateTickEvent -= tickable.LateTick;
        
        public void OnDestroy() => ServiceLocator.Unregister<UpdateSystem>();

        public void Tick(float deltaTime)
        {
            OnTickEvent?.Invoke(deltaTime);
        }
        
        public void LateTick(float deltaTime)
        {
            OnLateTickEvent?.Invoke(deltaTime);
        }
    }
}