using System.Collections.Generic;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using JellySort.Events;
using JellySort.Managers;
using UnityEngine;

namespace JellySort.Gameplay.HexaStack
{
    public class StackSpawner : MonoBehaviour
    {
        [SerializeField] private DraggableStack _draggableStackPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        
        private List<DraggableStack> _activeStacks = new List<DraggableStack>();
        private PoolManager _poolManager;

        public int ActiveStacksCount => _activeStacks.Count;

        private void Start()
        {
            _poolManager = ServiceLocator.Get<PoolManager>();
            if (_poolManager != null)
            {
                _poolManager.CreatePool("DraggableStack", _draggableStackPrefab, 3);
            }
            
            EventBus.Subscribe<StackSpawnRequestedEvent>(OnSpawnRequested);
            EventBus.Subscribe<StackEndDragEvent>(OnStackDropped);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<StackSpawnRequestedEvent>(OnSpawnRequested);
            EventBus.Unsubscribe<StackEndDragEvent>(OnStackDropped);
        }

        private void OnSpawnRequested(StackSpawnRequestedEvent evt)
        {
            SpawnStacks();
        }

        private void OnStackDropped(StackEndDragEvent evt)
        {
            if (evt.TargetNode != null)
            {
                _activeStacks.Remove(evt.Stack);
                
                if (_activeStacks.Count == 0)
                {
                    SpawnStacks();
                }
            }
        }

        public void SpawnStacks()
        {
            if (_poolManager == null) return;
            
            foreach (var stack in _activeStacks)
            {
                _poolManager.Despawn("DraggableStack", stack);
            }
            _activeStacks.Clear();

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                var stackObj = _poolManager.Spawn<DraggableStack>("DraggableStack", _spawnPoints[i].position, Quaternion.identity);
                stackObj.transform.SetParent(this.transform);
                
                List<HexaItem> items = GenerateRandomItems(Random.Range(2, 6));
                stackObj.Setup(items);
                
                _activeStacks.Add(stackObj);
            }

            EventBus.Publish(new StackSpawnCompletedEvent());
        }

        private List<HexaItem> GenerateRandomItems(int count)
        {
            List<HexaItem> items = new List<HexaItem>();
            
            var currentLevel = ServiceLocator.Get<LevelManager>().CurrentLevel;
            var allowedColors = currentLevel.AllowedColors;
            
            if (allowedColors == null || allowedColors.Count < 2)
            {
                Debug.LogError("Cần cấu hình ít nhất 2 màu trong AllowedColors của LevelSetupSO!");
                return items;
            }
            
            HexaColor color1 = allowedColors[Random.Range(0, allowedColors.Count)];
            HexaColor color2;
            do {
                color2 = allowedColors[Random.Range(0, allowedColors.Count)];
            } while (color1 == color2);
            
            int firstColorCount = Random.Range(1, count); 

            for (int i = 0; i < count; i++)
            {
                var item = _poolManager.Spawn<HexaItem>("HexaItem", Vector3.zero, Quaternion.identity);
                HexaColor chosenColor = (i < firstColorCount) ? color1 : color2;
                item.Setup(chosenColor);
                items.Add(item);
            }
            return items;
        }
    }
}
