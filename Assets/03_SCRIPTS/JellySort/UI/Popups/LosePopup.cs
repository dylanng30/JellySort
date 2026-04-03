using Dylanng.Core;
using Dylanng.Core.UI.Popups;
using JellySort.Events;
using JellySort.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.UI.Popups
{
    public class LosePopup : UIPopup
    {
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button homeButton;
        
        [SerializeField] private TextMeshProUGUI _reasonText;

        public void Open(string reason)
        {
            base.Show();
            
            if (_reasonText)
            {
                _reasonText.text = reason;
            }

            if (_retryButton != null)
            {
                _retryButton.onClick.RemoveListener(OnRetryClicked);
                _retryButton.onClick.AddListener(OnRetryClicked);
            }

            if (homeButton != null)
            {
                homeButton.onClick.RemoveListener(ReturnToMainMenu);
                homeButton.onClick.AddListener(ReturnToMainMenu);
            }
        }

        private void OnRetryClicked()
        {
            var saveLoadManager = ServiceLocator.Get<SaveLoadManager>();
            if (saveLoadManager != null)
            {
                int currentLevel = saveLoadManager.Data.CurrentLevel;
                EventBus.Publish(new LevelStartRequestedEvent
                {
                    LevelId = currentLevel
                });
            }
            Close();
        }
        
        private void ReturnToMainMenu()
        {
            EventBus.Publish(new ReturnToMainMenuRequestedEvent());
            Close();
        }
    }
}
