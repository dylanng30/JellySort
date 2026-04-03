using System.Collections.Generic;
using Dylanng.Core;
using Dylanng.Core.Base;
using Dylanng.Core.Pooling;
using JellySort.Events;
using JellySort.UI;
using UnityEngine;

namespace JellySort.Managers
{
    public class VFXManager : ManagerBase
    {
        [Header("UIFloatingText")]
        [SerializeField] private UIFloatingText _floatingTextPrefab;
        [SerializeField] private Vector2 floatingTextSpawner;
        [SerializeField] private RectTransform _canvasTransform;

        private PoolManager _poolManager;

        public override void Initialize()
        {
            _poolManager = ServiceLocator.Get<PoolManager>();
            if (_poolManager != null && _floatingTextPrefab != null)
            {
                _poolManager.CreatePool("UIFloatingText", _floatingTextPrefab, 5);
            }

            EventBus.Subscribe<ComboAchievedEvent>(OnComboAchieved);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.Unsubscribe<ComboAchievedEvent>(OnComboAchieved);
        }

        private void OnComboAchieved(ComboAchievedEvent evt)
        {
            if (_poolManager == null || _canvasTransform == null) return;

            string text = evt.ComboCount > 1 
                ? $"Combo x{evt.ComboCount}!\n+{evt.PointsEarned}" 
                : $"+{evt.PointsEarned}";
            
            var floatingText = _poolManager.Spawn<UIFloatingText>("UIFloatingText", Vector3.zero, Quaternion.identity);
            if (floatingText != null)
            {
                Color textColor = GetComboColor(evt.ComboCount, floatingText.ComboColors);
                floatingText.transform.SetParent(_canvasTransform, false);

                var rect = floatingText.Rect;
                rect.anchoredPosition = floatingTextSpawner;
                
                floatingText.Setup(text, textColor);
            }
        }

        private Color GetComboColor(int comboCount, List<Color> comboColors)
        {
            if (comboCount >= comboColors.Count) return comboColors[comboColors.Count - 1];
            return comboColors[comboCount - 1];
        }
    }
}