using System.Collections.Generic;
using UnityEngine;
using Dylanng.Core;
using Dylanng.Core.Base;
using JellySort.Data;
using JellySort.Events;
using JellySort.Gameplay.Boosters;
using JellySort.Gameplay.Grid;
using JellySort.Gameplay.HexaStack;

namespace JellySort.Managers
{
    public class BoosterManager : ManagerBase
    {
        [SerializeField] private List<BaseBooster> _availableBoosters;
        
        private Dictionary<BoosterType, BaseBooster> _boosterDict = new Dictionary<BoosterType, BaseBooster>();

        private BaseBooster _activeBooster;
        private bool _isWaitingForTarget;

        public override void Initialize()
        {
            ServiceLocator.Register<BoosterManager>(this);

            foreach (var booster in _availableBoosters)
            {
                if (booster != null && !_boosterDict.ContainsKey(booster.BoosterType))
                {
                    _boosterDict.Add(booster.BoosterType, booster);
                }
            }

            EventBus.Subscribe<BoosterActivatedEvent>(OnBoosterActivated);
            EventBus.Subscribe<BoosterCanceledEvent>(OnBoosterCanceled);
            EventBus.Subscribe<TargetNodeSelectedForBoosterEvent>(OnTargetNodeSelected);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ServiceLocator.Unregister<BoosterManager>();
            
            EventBus.Unsubscribe<BoosterActivatedEvent>(OnBoosterActivated);
            EventBus.Unsubscribe<BoosterCanceledEvent>(OnBoosterCanceled);
            EventBus.Unsubscribe<TargetNodeSelectedForBoosterEvent>(OnTargetNodeSelected);
        }

        private void OnBoosterActivated(BoosterActivatedEvent evt)
        {
            if (_isWaitingForTarget) 
            {
                CancelActiveBooster();
                if (_activeBooster != null && _activeBooster.BoosterType == evt.Type)
                    return;
            }

            if (!_boosterDict.TryGetValue(evt.Type, out var boosterStrategy))
            {
                GameLogger.LogError($"BoosterManager: No strategy found for {evt.Type}");
                return;
            }

            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            if (saveManager.GetBoosterCount(evt.Type) <= 0)
            {
                GameLogger.LogWarning($"BoosterManager: Not enough boosters of type {evt.Type}");
                return;
            }

            _activeBooster = boosterStrategy;

            if (boosterStrategy.RequiresTargetSelection)
            {
                _isWaitingForTarget = true;
                EventBus.Publish(new BoosterTargetModeStateChangedEvent
                {
                    IsActive = true, 
                    ActiveBoosterType = _activeBooster.BoosterType
                });
            }
            else
            {
                ExecuteBooster(null);
            }
        }

        private void OnBoosterCanceled(BoosterCanceledEvent evt)
        {
            if (_isWaitingForTarget)
                CancelActiveBooster();
        }

        private void OnTargetNodeSelected(TargetNodeSelectedForBoosterEvent evt)
        {
            if (!_isWaitingForTarget || _activeBooster == null || HexaStackController.IsProcessingMerge) return;

            ExecuteBooster(evt.Node);
        }

        private void CancelActiveBooster()
        {
            _isWaitingForTarget = false;
            _activeBooster = null;
            EventBus.Publish(new BoosterTargetModeStateChangedEvent { IsActive = false });
        }

        private void ExecuteBooster(HexaNode targetNode)
        {
            _isWaitingForTarget = false;
            EventBus.Publish(new BoosterTargetModeStateChangedEvent { IsActive = false });
            
            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            saveManager.UpdateBoosterCount(_activeBooster.BoosterType, -1);
            saveManager.Save();
            
            var levelManager = ServiceLocator.Get<LevelManager>();
            BoosterContext context = new BoosterContext
            {
                Board = levelManager.GridBoard,
                TargetNode = targetNode
            };
            
            var executingBooster = _activeBooster;
            _activeBooster = null;
            
            executingBooster.Execute(context, OnBoosterCompleted);
        }

        private void OnBoosterCompleted()
        {
            EventBus.Publish(new BoosterCompletedEvent());
            var levelManager = ServiceLocator.Get<LevelManager>();
            if (levelManager != null)
            {
                levelManager.SaveActiveGame();
            }
        }
    }
}
