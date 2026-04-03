using UnityEngine;
using UnityEngine.UI;

namespace JellySort.Helpers
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleSpriteSwapper : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _targetImage;

        [Header("Sprites")]
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(UpdateSprite);
        }

        private void OnEnable()
        {
            if (_toggle != null)
            {
                UpdateSprite(_toggle.isOn);
            }
        }

        private void UpdateSprite(bool isOn)
        {
            if (_targetImage != null)
            {
                _targetImage.sprite = isOn ? _onSprite : _offSprite;
            }
        }

        private void OnDestroy()
        {
            if (_toggle != null)
            {
                _toggle.onValueChanged.RemoveListener(UpdateSprite);
            }
        }
    }
}