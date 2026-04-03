using System;
using Dylanng.Core;
using Dylanng.Core.UI;
using Dylanng.Core.UI.Popups;
using JellySort.Data;
using JellySort.Events;
using JellySort.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.UI.Popups
{
    [Serializable]
    public struct ConditionDataContainer
    {
        public GameObject Panel;
        public TextMeshProUGUI Text;
    }
    
    public class LevelInfoPopup : UIPopup
    {
        [SerializeField] private TextMeshProUGUI _levelTitleText;
        
        [Space(10)]
        [SerializeField] private ConditionDataContainer _targetScoreContainer;
        [SerializeField] private ConditionDataContainer _moveLimitContainer;
        [SerializeField] private ConditionDataContainer _timeLimitContainer;
        
        [Space(10)]
        [SerializeField] private Button _playButton;

        private int _currentLevel;

        public override void Initialize()
        {
            base.Initialize();
            _playButton.onClick.RemoveListener(OnPlayClicked);
            _playButton.onClick.AddListener(OnPlayClicked);
        }

        public override void Show()
        {
            base.Show();
            LoadLevelInformation();
        }

        private void LoadLevelInformation()
        {
            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            _currentLevel = saveManager.Data.CurrentLevel;

            if (_levelTitleText != null) 
                _levelTitleText.text = $"---LEVEL {_currentLevel}---";
            
            var levelManager = ServiceLocator.Get<LevelManager>();
            LevelSetupSO levelData = levelManager.GetLevelData(_currentLevel);

            if (levelData != null)
            {
                if (levelData.RequiredPoints > 0)
                {
                    _targetScoreContainer.Panel.SetActive(true);
                    _targetScoreContainer.Text.text = $"{levelData.RequiredPoints}";
                }
                else
                {
                    _targetScoreContainer.Panel.SetActive(false);
                }

                if (levelData.MovesLimit > 0)
                {
                    _moveLimitContainer.Panel.SetActive(true);
                    _moveLimitContainer.Text.text = $"{levelData.MovesLimit}";
                }
                else
                {
                    _moveLimitContainer.Panel.SetActive(false);
                }

                if (levelData.TimeLimit > 0)
                {
                    _timeLimitContainer.Panel.SetActive(true);
                    _timeLimitContainer.Text.text = $"{levelData.TimeLimit} seconds";
                }
                else
                {
                    _timeLimitContainer.Panel.SetActive(false);
                }
            }
        }

        private void OnPlayClicked()
        {
            EventBus.Publish(new LevelStartRequestedEvent
            {
                LevelId = _currentLevel,
                ForceRestart = false
            });
            
            Close();
        }
    }
}