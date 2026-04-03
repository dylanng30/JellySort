using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dylanng.Core;
using Dylanng.Core.Base;

namespace Dylanng.Managers
{
    public class SceneLoadManager : ManagerBase
    {
        public override void Initialize()
        {
            ServiceLocator.Register<SceneLoadManager>(this);
            GameLogger.Log("SceneLoadManager Initialized");
        }

        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            // TODO: Bạn có thể gọi UIManager mở Loading Screen tại đây

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Hoàn thành load, kích hoạt Scene
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Bắn sự kiện Scene đã load xong cho các hệ thống khác biết
            //EventBus<SceneLoadedEvent>.Publish(new SceneLoadedEvent { SceneName = sceneName });
        }
    }
}