using System.Collections.Generic;
using Dylanng.Core;
using Dylanng.Core.Base;
using Dylanng.Core.Pooling;
using JellySort.Data;
using JellySort.Events;
using JellySort.Gameplay.Grid;
using JellySort.Gameplay.HexaStack;
using UnityEngine;

namespace JellySort.Managers
{
    public class LevelManager : ManagerBase
    {
        [SerializeField] private MaterialSO _materialSo;
        [SerializeField] private HexaGridBoard _gridBoard;
        [SerializeField] private HexaItem _hexaItemPrefab;
        
        [Header("Level Config")]
        [SerializeField] private List<LevelSetupSO> _levels;
        

        public LevelSetupSO CurrentLevel { get; private set; }
        public int CurrentScore => _currentScore;
        public HexaGridBoard GridBoard => _gridBoard;

        private int _currentScore;

        public override void Initialize()
        {
            ServiceLocator.Register<LevelManager>(this);
            
            var poolManager = ServiceLocator.Get<PoolManager>();
            if (poolManager != null && _hexaItemPrefab != null)
            {
                poolManager.CreatePool("HexaItem", _hexaItemPrefab, initialSize: 50);
                GameLogger.Log("LevelManager: Khởi tạo HexaItem pool (size 50).");
            }

            EventBus.Subscribe<ComboAchievedEvent>(OnComboAchieved);
            EventBus.Subscribe<LevelStartRequestedEvent>(OnLevelStartRequested);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.Unsubscribe<ComboAchievedEvent>(OnComboAchieved);
            EventBus.Unsubscribe<LevelStartRequestedEvent>(OnLevelStartRequested);
            ServiceLocator.Unregister<LevelManager>();
        }

        private void OnLevelStartRequested(LevelStartRequestedEvent evt)
        {
            GenerateCurrentLevel(evt.ForceRestart);
        }

        public void GenerateCurrentLevel(bool forceRestart = false)
        {
            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            var saveData = saveManager.Data;

            if (_levels == null || _levels.Count == 0)
            {
                GameLogger.LogError("LevelManager: Danh sách _levels trống! Hãy gán LevelSetupSO vào Inspector.");
                return;
            }
            
            int levelIndex = saveData.CurrentLevel - 1;

            if (levelIndex >= _levels.Count)
            {
                levelIndex = levelIndex % _levels.Count;
            }
            
            CurrentLevel = _levels[levelIndex];
    
            if (CurrentLevel == null)
            {
                GameLogger.LogError($"LevelManager: LevelSetupSO tại index {levelIndex} bị null!");
                return;
            }
            
            _gridBoard.GenerateBoard(CurrentLevel);
            _currentScore = 0;

            if (!forceRestart && saveData.ActiveLevelState != null && saveData.ActiveLevelState.HasActiveGame && saveData.ActiveLevelState.LevelId == CurrentLevel.LevelId)
            {
                _currentScore = saveData.ActiveLevelState.CurrentScore;
                PopulateSavedHexaStacks(saveData.ActiveLevelState);
                GameLogger.Log($"LevelManager: Tiếp tục level {CurrentLevel.LevelId}");
            }
            else
            {
                PopulateInitialHexaStacks();
                saveData.ActiveLevelState.HasActiveGame = false;
                saveManager.Save();
                GameLogger.Log($"LevelManager: Bắt đầu level {CurrentLevel.LevelId}");
            }

            EventBus.Publish(new StackSpawnRequestedEvent());

            LevelLoadedEvent loadedEvt = new LevelLoadedEvent { LevelId = CurrentLevel.LevelId };
            EventBus.Publish(loadedEvt);

            EventBus.Publish(new ScoreChangedEvent 
            { 
                NewScore = _currentScore, 
                TargetScore = CurrentLevel.RequiredPoints 
            });
        }

        public void SaveActiveGame()
        {
            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            if (saveManager == null || CurrentLevel == null) return;

            var activeState = saveManager.Data.ActiveLevelState;
            activeState.HasActiveGame = true;
            activeState.LevelId = CurrentLevel.LevelId;
            activeState.CurrentScore = _currentScore;
            activeState.Nodes.Clear();

            foreach (var node in _gridBoard.GetAllNodes())
            {
                var nodeData = new SavedNodeData
                {
                    Col = node.Coordinates.ToOffset().x,
                    Row = node.Coordinates.ToOffset().y,
                    IsLocked = node.IsLocked,
                    StackColors = new List<HexaColor>()
                };

                foreach (var item in node.GetItems())
                {
                    nodeData.StackColors.Add(item.ColorType);
                }
                activeState.Nodes.Add(nodeData);
            }

            saveManager.Save();
            GameLogger.Log("LevelManager: Active game progress saved.");
        }

        private void OnComboAchieved(ComboAchievedEvent evt)
        {
            if (CurrentLevel == null) return;
            
            _currentScore += evt.PointsEarned;
    
            EventBus.Publish(new ScoreChangedEvent 
            { 
                NewScore = _currentScore, 
                TargetScore = CurrentLevel.RequiredPoints 
            });

            if (_currentScore >= CurrentLevel.RequiredPoints)
            {
                ServiceLocator.Get<SaveLoadManager>().Data.ActiveLevelState.HasActiveGame = false;
                ServiceLocator.Get<SaveLoadManager>().Save();
        
                EventBus.Publish(new LevelCompletedEvent 
                { 
                    LevelId = CurrentLevel.LevelId, 
                    Score = _currentScore,
                    RewardCoins = CurrentLevel.RewardCoins
                });
            }
        }

        private void PopulateInitialHexaStacks()
        {
            var poolManager = ServiceLocator.Get<PoolManager>();
            if (poolManager == null) return;

            foreach (var nodeSetup in CurrentLevel.Nodes)
            {
                if (nodeSetup.StackColors == null || nodeSetup.StackColors.Count == 0)
                    continue;

                HexaCoordinates coords = HexaCoordinates.FromOffset(nodeSetup.Col, nodeSetup.Row);
                HexaNode node = _gridBoard.GetNodeAt(coords);
                if (node != null) SpawnItemsOnNode(node, nodeSetup.StackColors);
            }
        }

        private void PopulateSavedHexaStacks(SavedLevelState savedState)
        {
            foreach (var nodeData in savedState.Nodes)
            {
                if (nodeData.StackColors == null || nodeData.StackColors.Count == 0)
                    continue;

                HexaCoordinates coords = HexaCoordinates.FromOffset(nodeData.Col, nodeData.Row);
                HexaNode node = _gridBoard.GetNodeAt(coords);
                if (node != null) SpawnItemsOnNode(node, nodeData.StackColors);
            }
        }

        private void SpawnItemsOnNode(HexaNode node, List<HexaColor> colors)
        {
            var poolManager = ServiceLocator.Get<PoolManager>();
            foreach (var color in colors)
            {
                var spawnedHexa = poolManager.Spawn<HexaItem>("HexaItem", Vector3.zero, Quaternion.identity);
                spawnedHexa.Setup(color);
                spawnedHexa.transform.position = node.GetTopPlacementPosition();
                node.AddToStack(spawnedHexa);
            }
        }

        public Material GetMaterialForColor(HexaColor color)
        {
            return _materialSo.GetMaterialForColor(color);
        }
        
        public LevelSetupSO GetLevelData(int levelNumber)
        {
            if (_levels == null || _levels.Count == 0)
            {
                GameLogger.LogError("LevelManager: Danh sách levels trống!");
                return null;
            }

            int levelIndex = levelNumber - 1;
            
            if (levelIndex >= _levels.Count)
            {
                levelIndex = levelIndex % _levels.Count;
            }

            return _levels[levelIndex];
        }
    }
}
