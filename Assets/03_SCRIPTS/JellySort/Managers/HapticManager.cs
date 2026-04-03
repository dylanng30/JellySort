using UnityEngine;
using Dylanng.Core;
using Dylanng.Core.Base;

namespace JellySort.Managers
{
    public class HapticManager : ManagerBase
    {
        public override void Initialize()
        {
            ServiceLocator.Register<HapticManager>(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ServiceLocator.Unregister<HapticManager>();
        }

        public void PlayHaptic()
        {
            var saveData = ServiceLocator.Get<SaveLoadManager>()?.Data;
            if (saveData != null && saveData.IsHapticOn)
            {
                //Handheld.Vibrate();
            }
        }
    }
}
