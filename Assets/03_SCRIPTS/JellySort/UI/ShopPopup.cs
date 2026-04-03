using System.Collections.Generic;
using Dylanng.Core;
using Dylanng.Core.UI;
using Dylanng.Core.UI.Popups;
using JellySort.Data;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.UI
{
    public class ShopPopup : UIPopup
    {
        [Header("Shop Config")]
        [SerializeField] private ShopConfigSO _shopConfig;
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private UIShopItem _itemPrefab;
        [SerializeField] private Button _closeButton;

        private List<UIShopItem> _spawnedItems = new List<UIShopItem>();

        public override void Show()
        {
            base.Show();
            _closeButton.onClick.RemoveListener(OnCloseClicked);
            _closeButton.onClick.AddListener(OnCloseClicked);
            PopulateItems();
        }

        private void PopulateItems()
        {
            foreach (var item in _spawnedItems)
            {
                if (item != null) Destroy(item.gameObject);
            }
            _spawnedItems.Clear();
            
            if (_shopConfig != null && _itemPrefab != null)
            {
                foreach (var itemData in _shopConfig.Items)
                {
                    UIShopItem newItem = Instantiate(_itemPrefab, _itemsContainer);
                    newItem.Setup(itemData);
                    _spawnedItems.Add(newItem);
                }
            }
        }

        private void OnCloseClicked()
        {
            ServiceLocator.Get<UIManager>().CloseCurrentPopup();
        }
    }
}
