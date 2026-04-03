using Dylanng.Core;
using JellySort.Data;
using JellySort.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.UI
{
    public class UIShopItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _buyButton;

        private ShopItemData _data;

        public void Setup(ShopItemData data)
        {
            _data = data;
            if (_nameText != null) _nameText.text = data.ProductName;
            if (_iconImage != null) _iconImage.sprite = data.Icon;
            if (_priceText != null) _priceText.text = data.Price.ToString();

            _buyButton.onClick.RemoveAllListeners();
            _buyButton.onClick.AddListener(OnBuyClicked);
        }

        private void OnBuyClicked()
        {
            var economyManager = ServiceLocator.Get<EconomyManager>();
            if (economyManager != null)
            {
                economyManager.TryPurchaseBooster(_data.BoosterType, _data.Price, _data.Amount);
            }
        }
    }
}
