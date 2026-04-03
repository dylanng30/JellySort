using UnityEngine;
using UnityEngine.UI;
using Dylanng.Core;
using Dylanng.Core.UI.Popups;
using Dylanng.Managers;
using JellySort.Managers;

namespace JellySort.UI.Popups
{
    public class SettingsPopup : UIPopup
    {
        [Header("Settings Toggles")]
        [SerializeField] private Toggle _soundToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _hapticToggle;
        [SerializeField] private Button _closeButton;

        public override void Initialize()
        {
            base.Initialize();
            _soundToggle.onValueChanged.AddListener(OnSoundToggled);
            _musicToggle.onValueChanged.AddListener(OnMusicToggled);
            _hapticToggle.onValueChanged.AddListener(OnHapticToggled);
            _closeButton.onClick.AddListener(Close);
        }

        public override void Show()
        {
            base.Show();
            var saveData = ServiceLocator.Get<SaveLoadManager>().Data;
            
            _soundToggle.isOn = saveData.IsSoundOn;
            _musicToggle.isOn = saveData.IsMusicOn;
            _hapticToggle.isOn = saveData.IsHapticOn;
        }

        private void OnSoundToggled(bool isOn)
        {
            ServiceLocator.Get<SaveLoadManager>().ToggleSound(isOn);
            ServiceLocator.Get<AudioManager>().SetSFXMute(!isOn);
        }

        private void OnMusicToggled(bool isOn)
        {
            ServiceLocator.Get<SaveLoadManager>().ToggleMusic(isOn);
            ServiceLocator.Get<AudioManager>().SetMusicMute(!isOn);
        }

        private void OnHapticToggled(bool isOn)
        {
            ServiceLocator.Get<SaveLoadManager>().ToggleHaptic(isOn);
            if (isOn) ServiceLocator.Get<HapticManager>()?.PlayHaptic();
        }
    }
}
