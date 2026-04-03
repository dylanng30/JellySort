using UnityEngine;
using Dylanng.Core.Base;

namespace Dylanng.Core.Systems
{
    public class PauseSystem : SystemBase
    {
        public bool IsPaused { get; private set; }

        public override void Initialize()
        {
            ServiceLocator.Register<PauseSystem>(this);
            IsPaused = false;
        }

        public void TogglePause()
        {
            SetPause(!IsPaused);
        }

        public void SetPause(bool pause)
        {
            if (IsPaused == pause) return;

            IsPaused = pause;
            Time.timeScale = IsPaused ? 0f : 1f;

            //EventBus<GamePausedEvent>.Publish(new GamePausedEvent { IsPaused = IsPaused });
        }
    }
}