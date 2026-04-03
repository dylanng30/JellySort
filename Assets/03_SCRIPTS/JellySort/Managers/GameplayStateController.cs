using Dylanng.Core;
using Dylanng.Core.Base;
using Dylanng.Core.State;
using JellySort.Events;
using JellySort.UI;
using JellySort.UI.Popups;
using UnityEngine;
using UIManager = Dylanng.Core.UI.UIManager;

namespace JellySort.Managers
{
    public class GameplayStateController : ManagerBase
    {
        private StateMachine _stateMachine;
        private UIManager _uiManager;

        public override void Initialize()
        {
            ServiceLocator.Register<GameplayStateController>(this);
            _stateMachine = new StateMachine();
            _uiManager = ServiceLocator.Get<UIManager>();
            
            var setupState = new SetupState(this);
            _stateMachine.Initialize(setupState);
            
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Subscribe<LevelFailedEvent>(OnLevelFailed);
            EventBus.Subscribe<LevelStartRequestedEvent>(OnLevelStartRequested);
            EventBus.Subscribe<ReturnToMainMenuRequestedEvent>(OnReturnToMainMenu);
            EventBus.Subscribe<RestartLevelRequestedEvent>(OnRestartLevel);
            EventBus.Subscribe<ShopOpenRequestedEvent>(OnShopOpenRequested);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ServiceLocator.Unregister<GameplayStateController>();
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Unsubscribe<LevelFailedEvent>(OnLevelFailed);
            EventBus.Unsubscribe<LevelStartRequestedEvent>(OnLevelStartRequested);
            EventBus.Unsubscribe<ReturnToMainMenuRequestedEvent>(OnReturnToMainMenu);
            EventBus.Unsubscribe<RestartLevelRequestedEvent>(OnRestartLevel);
            EventBus.Unsubscribe<ShopOpenRequestedEvent>(OnShopOpenRequested);
        }
        
        private void OnShopOpenRequested(ShopOpenRequestedEvent evt)
        {
            _uiManager.OpenPopup<ShopPopup>();
        }

        private void OnReturnToMainMenu(ReturnToMainMenuRequestedEvent evt)
        {
            ServiceLocator.Get<LevelManager>()?.SaveActiveGame();
            _stateMachine.TransitionTo(new SetupState(this));
        }

        private void OnRestartLevel(RestartLevelRequestedEvent evt)
        {
            var livesManager = ServiceLocator.Get<LivesManager>();
            if (livesManager != null && !livesManager.HasLives())
            {
                GameLogger.LogWarning("No lives left! Cannot restart game.");
                return;
            }

            int currentLevel = ServiceLocator.Get<SaveLoadManager>().Data.CurrentLevel;
            EventBus.Publish(new LevelStartRequestedEvent { LevelId = currentLevel, ForceRestart = true });
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        private void OnLevelStartRequested(LevelStartRequestedEvent evt)
        {
            _stateMachine.TransitionTo(new PlayState(this));
        }

        private void OnLevelCompleted(LevelCompletedEvent evt)
        {
            _stateMachine.TransitionTo(new EndState(this, true, evt.LevelId,evt.Score, evt.RewardCoins));
        }

        private void OnLevelFailed(LevelFailedEvent evt)
        {
            _stateMachine.TransitionTo(new EndState(this, false, evt.FailReason));
        }
        
        public class SetupState : StateBase
        {
            private GameplayStateController _owner;
            public SetupState(GameplayStateController owner) => _owner = owner;

            public override void Enter()
            {
                _owner._uiManager.OpenScreen<MainMenuScreen>();
            }
        }

        public class PlayState : StateBase
        {
            private GameplayStateController _owner;
            private float _timeRemaining;
            private int _currentMoves;
            private bool _isTimeLevel;
            private bool _isMovesLevel;
            private bool _isPlaying;

            public PlayState(GameplayStateController owner) => _owner = owner;

            public override void Enter()
            {
                _owner._uiManager.OpenScreen<GameplayHUD>();
                
                var levelManager = ServiceLocator.Get<LevelManager>();
                var currentLevel = levelManager.CurrentLevel;

                if (currentLevel != null)
                {
                    if (currentLevel.TimeLimit > 0)
                    {
                        _isTimeLevel = true;
                        _timeRemaining = currentLevel.TimeLimit;
                    }
                    else if (currentLevel.MovesLimit > 0)
                    {
                        _isMovesLevel = true;
                        _currentMoves = currentLevel.MovesLimit;
                    }
                }
                
                _isPlaying = true;
                EventBus.Subscribe<StackEndDragEvent>(OnMoveMade);
                GameLogger.Log("Gameplay Started.");
            }

            public override void Update()
            {
                if (!_isPlaying || !_isTimeLevel) return;

                float prevTime = _timeRemaining;
                _timeRemaining -= Time.deltaTime;

                int timeInt = Mathf.CeilToInt(_timeRemaining);
                if (timeInt != Mathf.CeilToInt(prevTime))
                {
                    EventBus.Publish(new TimeChangedEvent 
                    { 
                        TimeRemaining = timeInt, 
                        TimeLimit = ServiceLocator.Get<LevelManager>().CurrentLevel.TimeLimit 
                    });
                }

                if (_timeRemaining <= 0)
                {
                    _isPlaying = false;
                    EventBus.Publish(new LevelFailedEvent
                    {
                        FailReason = "Time's Up!"
                    });
                }
            }

            private void OnMoveMade(StackEndDragEvent evt)
            {
                if (!_isPlaying || !_isMovesLevel) return;

                _currentMoves--;
                EventBus.Publish(new MovesChangedEvent 
                { 
                    CurrentMoves = _currentMoves, 
                    MoveLimit = ServiceLocator.Get<LevelManager>().CurrentLevel.MovesLimit 
                });

                if (_currentMoves <= 0)
                {
                    _isPlaying = false;
                    EventBus.Publish(new LevelFailedEvent
                    {
                        FailReason = "Out of Moves!"
                    });
                }
            }

            public override void Exit()
            {
                EventBus.Unsubscribe<StackEndDragEvent>(OnMoveMade);
            }
        }

        public class EndState : StateBase
        {
            private GameplayStateController _owner;
            private int _levelId;
            private bool _isWin;
            private int _score;
            private int _rewardCoins;
            private string _failReason;

            public EndState(GameplayStateController owner, bool isWin, int levelId, int score, int rewardCoins)
            {
                _owner = owner;
                _levelId = levelId;
                _isWin = isWin;
                _score = score;
                _rewardCoins = rewardCoins;
            }

            public EndState(GameplayStateController owner, bool isWin, string failReason = "")
            {
                _owner = owner;
                _isWin = isWin;
                _failReason = failReason;
            }

            public override void Enter()
            {
                if (_isWin)
                {
                    var winPopup = _owner._uiManager.OpenPopup<WinPopup>();
                    winPopup.Open(_levelId, _score, _rewardCoins);
                }
                else
                {
                    var losePopup = _owner._uiManager.OpenPopup<LosePopup>();
                    losePopup.Open(_failReason);
                }
                
                GameLogger.Log("Win: " + _isWin);
            }
        }
    }
}
