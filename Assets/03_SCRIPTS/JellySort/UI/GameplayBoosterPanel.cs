using System.Collections.Generic;
using UnityEngine;
using Dylanng.Core;
using JellySort.Data;
using JellySort.Events;
using JellySort.Managers;

namespace JellySort.UI
{
    public class GameplayBoosterPanel : MonoBehaviour
    {
        [SerializeField] private List<UIBoosterButton> _buttons;

        private void OnEnable()
        {
            EventBus.Subscribe<LevelLoadedEvent>(OnLevelLoaded);
            EventBus.Subscribe<BoosterTargetModeStateChangedEvent>(OnBoosterStateChanged);
            EventBus.Subscribe<BoosterCompletedEvent>(OnBoosterCompleted);
            
            RefreshCounts();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelLoadedEvent>(OnLevelLoaded);
            EventBus.Unsubscribe<BoosterTargetModeStateChangedEvent>(OnBoosterStateChanged);
            EventBus.Unsubscribe<BoosterCompletedEvent>(OnBoosterCompleted);
        }

        private void OnLevelLoaded(LevelLoadedEvent evt)
        {
            RefreshCounts();
        }

        private void RefreshCounts()
        {
            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            if (saveManager == null) return;

            foreach (var btn in _buttons)
            {
                if (btn != null)
                {
                    btn.UpdateCount(saveManager.GetBoosterCount(btn.BoosterType));
                }
            }
        }

        private void OnBoosterStateChanged(BoosterTargetModeStateChangedEvent evt)
        {
            foreach (var btn in _buttons)
            {
                if (btn != null)
                {
                    btn.SetSelectedState(evt.IsActive && evt.ActiveBoosterType == btn.BoosterType);
                }
            }
        }

        private void OnBoosterCompleted(BoosterCompletedEvent evt)
        {
            RefreshCounts();
        }
    }
}
