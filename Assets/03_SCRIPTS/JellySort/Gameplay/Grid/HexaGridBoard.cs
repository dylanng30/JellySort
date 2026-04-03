using System.Collections.Generic;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using UnityEngine;
using JellySort.Data;

namespace JellySort.Gameplay.Grid
{
    public class HexaGridBoard : MonoBehaviour
    {
        [SerializeField] private HexaNode _nodePrefab;
        [SerializeField] private float _hexRadius = 1f;

        private Dictionary<HexaCoordinates, HexaNode> _gridNodes = new Dictionary<HexaCoordinates, HexaNode>();

        private readonly int[,] directions = new int[,] {
            { 1, 0, -1 }, { 1, -1, 0 }, { 0, -1, 1 },
            { -1, 0, 1 }, { -1, 1, 0 }, { 0, 1, -1 }
        };
        
        public void GenerateBoard(LevelSetupSO levelData)
        {
            ClearBoard();

            foreach (var nodeSetup in levelData.Nodes)
            {
                HexaCoordinates coords = HexaCoordinates.FromOffset(nodeSetup.Col, nodeSetup.Row);

                HexaNode node = Instantiate(_nodePrefab, this.transform);
                node.transform.position = coords.ToWorldPosition(_hexRadius);
                node.Setup(coords, nodeSetup.IsLocked, nodeSetup.IsIceGrid);

                _gridNodes.TryAdd(coords, node);
            }
        }

        public void ClearBoard()
        {
            var poolManager = ServiceLocator.Get<PoolManager>();

            foreach (var node in _gridNodes.Values)
            {
                if (node != null)
                {
                    if (poolManager != null && node.StackCount > 0)
                    {
                        foreach (var item in node.GetItems())
                        {
                            item.transform.SetParent(null);
                            poolManager.Despawn("HexaItem", item);
                        }
                        node.ClearStack();
                    }
                    
                    Destroy(node.gameObject);
                }
            }
            _gridNodes.Clear();
        }

        public HexaNode GetNodeAt(HexaCoordinates coords)
        {
            if (_gridNodes.TryGetValue(coords, out var node))
                return node;
            return null;
        }
        
        public IEnumerable<HexaNode> GetNeighbors(HexaCoordinates coords)
        {
            for (int i = 0; i < 6; i++)
            {
                var neighborCoords = new HexaCoordinates(
                    coords.Q + directions[i, 0],
                    coords.R + directions[i, 1],
                    coords.S + directions[i, 2]
                );

                if (_gridNodes.TryGetValue(neighborCoords, out var neighborNode))
                {
                    yield return neighborNode;
                }
            }
        }

        public int GetEmptyNodesCount()
        {
            int count = 0;
            foreach (var node in _gridNodes.Values)
            {
                if (node.StackCount == 0 && !node.IsLocked) count++;
            }
            return count;
        }

        public IEnumerable<HexaNode> GetAllNodes()
        {
            return _gridNodes.Values;
        }
    }
}
