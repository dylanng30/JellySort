using Dylanng.Core;
using JellySort.Events;
using JellySort.Managers;
using TMPro;
using UnityEngine;

namespace JellySort.UI
{
    public class UICoinDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinText;

        private void OnEnable()
        {
            EventBus.Subscribe<CoinsChangedEvent>(OnCoinsChanged);
            RefreshDisplay();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CoinsChangedEvent>(OnCoinsChanged);
        }

        private void OnCoinsChanged(CoinsChangedEvent evt)
        {
            UpdateDisplay(evt.NewAmount);
        }

        private void RefreshDisplay()
        {
            var economyManager = ServiceLocator.Get<EconomyManager>();
            if (economyManager != null)
                UpdateDisplay(economyManager.CurrentCoins);
        }

        private void UpdateDisplay(int amount)
        {
            if (_coinText != null)
                _coinText.text = amount.ToString();
        }
    }
}
