using Dylanng.Core;
using Dylanng.Core.UI.Popups;
using JellySort.Events;
using JellySort.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.UI.Popups
{
    public class WinPopup : UIPopup
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button closeButton;
        
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _rewardCoinText;

        public void Open(int levelId, int score, int rewardCoins)
        {
            if(_levelText) _levelText.text = $"---LEVEL {levelId.ToString()}---";
            if(_scoreText) _scoreText.text = score.ToString();
            if(_rewardCoinText) _rewardCoinText.text = rewardCoins.ToString();

            if (_nextLevelButton)
            {
                _nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
                _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            }

            if (closeButton)
            {
                closeButton.onClick.RemoveListener(ReturnToMainMenu);
                closeButton.onClick.AddListener(ReturnToMainMenu);
            }
        }

        private void OnNextLevelClicked()
        {
            var saveLoadManager = ServiceLocator.Get<SaveLoadManager>();
            if (saveLoadManager != null)
            {
                int nextLevel = saveLoadManager.Data.CurrentLevel;
                EventBus.Publish(new LevelStartRequestedEvent 
                {
                    LevelId = nextLevel
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
