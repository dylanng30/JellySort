using Dylanng.Core;
using Dylanng.Core.UI;
using Dylanng.Core.UI.Screens;
using Dylanng.Data;
using Dylanng.Managers;
using JellySort.Events;
using JellySort.Managers;
using JellySort.UI.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.UI
{
    public class MainMenuScreen : UIScreen
    {
        [Header("---HEADER---")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button buyCoinButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _levelText;

        public override void Show()
        {
            base.Show();
            int currentLevel = ServiceLocator.Get<SaveLoadManager>().Data.CurrentLevel;
            _levelText.text = $"LEVEL {currentLevel}";
            
            _playButton.onClick.RemoveListener(OnPlayClicked);
            _playButton.onClick.AddListener(OnPlayClicked);

            settingsButton.onClick.RemoveListener(OnSettingsClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            
            shopButton.onClick.RemoveListener(OpenShop);
            shopButton.onClick.AddListener(OpenShop);
            
            buyCoinButton.onClick.RemoveListener(OpenShop);
            buyCoinButton.onClick.AddListener(OpenShop);
        }

        private void OnPlayClicked()
        {
            var livesManager = ServiceLocator.Get<LivesManager>();
            if (livesManager != null && !livesManager.HasLives())
            {
                GameLogger.LogWarning("No lives left! Cannot start game.");
                return;
            }
            
            ServiceLocator.Get<UIManager>().OpenPopup<LevelInfoPopup>();
            Hide();
        }

        private void OpenShop()
        {
            EventBus.Publish(new ShopOpenRequestedEvent());
        }
        
        private void OnSettingsClicked()
        {
            ServiceLocator.Get<UIManager>().OpenPopup<SettingsPopup>();
        }
    }
}
