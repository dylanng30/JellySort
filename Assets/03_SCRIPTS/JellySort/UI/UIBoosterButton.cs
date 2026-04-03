using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JellySort.Data;
using Dylanng.Core;
using JellySort.Events;

namespace JellySort.UI
{
    public class UIBoosterButton : MonoBehaviour
    {
        public BoosterType BoosterType;
        
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _countText;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Setup(BoosterType type)
        {
            BoosterType = type;
        }

        public void UpdateCount(int count)
        {
            if (_countText != null)
                _countText.text = count.ToString();
            
            if (_button != null)
                _button.interactable = count > 0;
        }

        public void SetSelectedState(bool isSelected)
        {
            transform.localScale = isSelected ? new Vector3(1.5f, 1.5f, 1.5f) : Vector3.one;
        }

        private void OnClick()
        {
            Debug.Log($"Booster {BoosterType} button is clicked");
            EventBus.Publish(new BoosterActivatedEvent
            {
                Type = BoosterType
            });
        }
    }
}
