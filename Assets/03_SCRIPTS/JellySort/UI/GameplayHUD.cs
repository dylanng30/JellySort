using Dylanng.Core;
using Dylanng.Core.UI.Screens;
using JellySort.Events;
using JellySort.Managers;
using JellySort.UI.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.UI
{
    public class GameplayHUD : UIScreen
    {
        [SerializeField] private TextMeshProUGUI _scoreLeftText;
        [SerializeField] private TextMeshProUGUI _limitText;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button shopButton;

        private int _currentScore;
        private int _targetScore;

        public override void Show()
        {
            base.Show();
            
            _pauseButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.AddListener(OnPauseClicked);
            
            shopButton.onClick.RemoveAllListeners();
            shopButton.onClick.AddListener(OpenShop);
            
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Subscribe<LevelLoadedEvent>(OnLevelLoaded);
            EventBus.Subscribe<MovesChangedEvent>(OnMovesChanged);
            EventBus.Subscribe<TimeChangedEvent>(OnTimeChanged);
            
            RefreshCurrentStatus();
        }

        protected override void OnDestroy()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
            EventBus.Unsubscribe<LevelLoadedEvent>(OnLevelLoaded);
            EventBus.Unsubscribe<MovesChangedEvent>(OnMovesChanged);
            EventBus.Unsubscribe<TimeChangedEvent>(OnTimeChanged);
            base.OnDestroy();
        }

        private void RefreshCurrentStatus()
        {
            var levelManager = ServiceLocator.Get<LevelManager>();
            if (levelManager != null && levelManager.CurrentLevel != null)
            {
                _targetScore = levelManager.CurrentLevel.RequiredPoints;
                _currentScore = levelManager.CurrentScore;
                UpdateScoreUI();
                
                var currentLevel = levelManager.CurrentLevel;
                if (currentLevel.TimeLimit > 0)
                    _limitText.text = "TIME: " + currentLevel.TimeLimit;
                else if (currentLevel.MovesLimit > 0)
                    _limitText.text = "MOVES: " + currentLevel.MovesLimit;
                else
                    _limitText.text = "";
            }
        }

        private void OnLevelLoaded(LevelLoadedEvent evt)
        {
            var levelManager = ServiceLocator.Get<LevelManager>();
            if (levelManager != null && levelManager.CurrentLevel != null)
            {
                var currentLevel = levelManager.CurrentLevel;
                _targetScore = currentLevel.RequiredPoints;
                _currentScore = 0;
                
                UpdateScoreUI();
            }
        }

        private void OnScoreChanged(ScoreChangedEvent evt)
        {
            _currentScore = evt.NewScore;
            _targetScore = evt.TargetScore;
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            int scoreLeft = _targetScore - _currentScore;
            if (scoreLeft < 0) scoreLeft = 0;
            
            _scoreLeftText.text = scoreLeft.ToString();
        }

        private void OnMovesChanged(MovesChangedEvent evt)
        {
            _limitText.text = "MOVES: " + evt.CurrentMoves;
        }

        private void OnTimeChanged(TimeChangedEvent evt)
        {
            _limitText.text = "TIME: " + evt.TimeRemaining;
        }

        private void OnPauseClicked()
        {
            ServiceLocator.Get<Dylanng.Core.UI.UIManager>().OpenPopup<GameplaySettingsPopup>();
        }
        
        private void OpenShop()
        {
            Time.timeScale = 0;
            EventBus.Publish(new ShopOpenRequestedEvent());
        }
    }
}