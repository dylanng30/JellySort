using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using Dylanng.Data;
using Dylanng.Managers;
using JellySort.Events;
using JellySort.Gameplay.Grid;
using UnityEngine;

namespace JellySort.Gameplay.HexaStack
{
    public class HexaStackController : MonoBehaviour
    {
        [SerializeField] private HexaGridBoard _gridBoard;
        [SerializeField] private ParticleSystem _popVfxPrefab;
        [SerializeField] private PoolableVFX _iceBreakVfxPrefab;
        
        public static bool IsProcessingMerge { get; private set; }

        private void OnEnable()
        {
            EventBus.Subscribe<StackEndDragEvent>(OnStackDropped);
            EventBus.Subscribe<ForceMergeCheckEvent>(OnForceMergeCheck);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<StackEndDragEvent>(OnStackDropped);
            EventBus.Unsubscribe<ForceMergeCheckEvent>(OnForceMergeCheck);
        }

        private void OnStackDropped(StackEndDragEvent evt)
        {
            if (evt.TargetNode == null) return;

            StartCoroutine(PlaceAndMergeRoutine(evt.Stack, evt.TargetNode));
        }

        private void OnForceMergeCheck(ForceMergeCheckEvent evt)
        {
            if (evt.TargetNode == null) return;
            StartCoroutine(ForceMergeRoutine(evt.TargetNode));
        }

        private IEnumerator ForceMergeRoutine(HexaNode targetNode)
        {
            IsProcessingMerge = true;
            yield return StartCoroutine(MergeLogicRoutine(targetNode));
            IsProcessingMerge = false;
        }

        private IEnumerator PlaceAndMergeRoutine(DraggableStack stack, HexaNode targetNode)
        {
            IsProcessingMerge = true;
            
            List<HexaItem> itemsToPlace = new List<HexaItem>(stack.Items);
            stack.Clear();

            foreach (var item in itemsToPlace)
            {
                Vector3 targetPos = targetNode.GetTopPlacementPosition();
                item.transform.SetParent(targetNode.transform);
                item.MoveTo(targetPos, 0.2f).OnComplete(() => 
                {
                    ServiceLocator.Get<AudioManager>()?.PlaySFX(SoundType.Gameplay_Pop);
                });
                targetNode.AddToStack(item);
                yield return new WaitForSeconds(0.05f);
            }
            
            ServiceLocator.Get<PoolManager>().Despawn("DraggableStack", stack);
            
            yield return new WaitForSeconds(0.2f);
            
            yield return StartCoroutine(MergeLogicRoutine(targetNode));
            
            StackSpawner spawner = Object.FindFirstObjectByType<StackSpawner>();
            if (spawner != null && spawner.ActiveStacksCount > 0)
            {
                if (_gridBoard.GetEmptyNodesCount() == 0)
                {
                    GameLogger.Log("No empty nodes left and spawner has active stacks. Level Failed.");
                    EventBus.Publish(new LevelFailedEvent
                    {
                        FailReason = "No space left!"
                    });
                }
            }

            IsProcessingMerge = false;
        }

        private IEnumerator MergeLogicRoutine(HexaNode startNode)
        {
            var poolManager = ServiceLocator.Get<PoolManager>();
            if (_iceBreakVfxPrefab != null) poolManager.CreatePool("IceBreakVFX", _iceBreakVfxPrefab, 5);
            
            List<HexaNode> nodesToProcess = new List<HexaNode>();
            nodesToProcess.Add(startNode);

            while (nodesToProcess.Count > 0)
            {
                HexaNode currentNode = nodesToProcess[0];
                nodesToProcess.RemoveAt(0);

                if (currentNode == null) continue;

                HexaItem topItem = currentNode.GetTopItem();
                if (topItem == null) continue;

                HexaColor targetColor = topItem.ColorType;
                bool hasMergedThisIteration = false;
                
                var neighbors = _gridBoard.GetNeighbors(currentNode.Coordinates);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.StackCount == 0 || neighbor.IsIceGrid) continue;
                    
                    if (neighbor.GetTopItem().ColorType == targetColor)
                    {
                        yield return StartCoroutine(PullColorFromNeighbor(neighbor, currentNode, targetColor));
                        
                        if (!nodesToProcess.Contains(neighbor))
                            nodesToProcess.Add(neighbor);

                        hasMergedThisIteration = true;
                    }
                }
                
                bool didPop = false;
                yield return StartCoroutine(CheckAndPopStack(currentNode, (popped) => didPop = popped));

                if (didPop)
                {
                    var popNeighbors = _gridBoard.GetNeighbors(currentNode.Coordinates);
                    foreach (var neighbor in popNeighbors)
                    {
                        if (neighbor.IsIceGrid)
                        {
                            bool isBroken = neighbor.DamageIce();
                            if (isBroken)
                            {
                                if (_iceBreakVfxPrefab != null)
                                {
                                    var vfx = poolManager.Spawn<PoolableVFX>("IceBreakVFX", neighbor.transform.position, Quaternion.identity);
                                    vfx.Setup("IceBreakVFX");
                                }
                        
                                EventBus.Publish(new IceBrokenEvent
                                {
                                    Node = neighbor, 
                                    Position = neighbor.transform.position
                                });
                                
                                if (neighbor.StackCount > 0 && !nodesToProcess.Contains(neighbor))
                                {
                                    nodesToProcess.Add(neighbor);
                                }
                            }
                        }
                    }
                }
                
                if (hasMergedThisIteration || didPop)
                {
                    if (!nodesToProcess.Contains(currentNode))
                        nodesToProcess.Add(currentNode);
                }
            }
        }

        private IEnumerator PullColorFromNeighbor(HexaNode fromNode, HexaNode toNode, HexaColor color)
        {
            List<HexaItem> itemsToMove = new List<HexaItem>();
            List<HexaItem> neighborItems = fromNode.GetItems();
            
            for (int i = neighborItems.Count - 1; i >= 0; i--)
            {
                if (neighborItems[i].ColorType == color)
                {
                    itemsToMove.Add(neighborItems[i]);
                }
                else
                {
                    break;
                }
            }
            
            foreach (var item in itemsToMove)
            {
                fromNode.RemoveFromStack(item);
                Vector3 targetPos = toNode.GetTopPlacementPosition();
                item.transform.SetParent(toNode.transform);
                
                item.MoveTo(targetPos, 0.25f).OnComplete(() => 
                {
                    ServiceLocator.Get<AudioManager>()?.PlaySFX(SoundType.Gameplay_Pop);
                });
                toNode.AddToStack(item);
                
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.25f);
        }

        private IEnumerator CheckAndPopStack(HexaNode node, System.Action<bool> onComplete)
        {
            var items = node.GetItems();
            if (items.Count == 0)
            {
                onComplete?.Invoke(false);
                yield break;
            }

            HexaColor topColor = items[^1].ColorType;
            int consecutiveCount = 0;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].ColorType == topColor) consecutiveCount++;
                else break;
            }

            if (consecutiveCount >= 10)
            {
                List<HexaItem> itemsToPop = new List<HexaItem>();
                for (int i = 0; i < consecutiveCount; i++)
                {
                    HexaItem itemToPop = items[^1];
                    itemsToPop.Add(itemToPop);
                    node.RemoveFromStack(itemToPop);
                }
                
                Sequence popSeq = DOTween.Sequence();
                foreach (var item in itemsToPop)
                {
                    popSeq.Join(item.transform.DOScale(Vector3.zero, 0.2f));
                }

                yield return popSeq.WaitForCompletion();
                
                if (_popVfxPrefab != null)
                {
                    var vfx = Instantiate(_popVfxPrefab, node.GetTopPlacementPosition(), Quaternion.identity);
                    Destroy(vfx.gameObject, 2f);
                }

                var poolManager = ServiceLocator.Get<PoolManager>();
                
                foreach (var item in itemsToPop)
                {
                    item.transform.localScale = Vector3.one;
                    poolManager.Despawn("HexaItem", item);
                }
                
                EventBus.Publish(new HexaMergeCompletedEvent
                {
                    PoppedCount = itemsToPop.Count,
                    MergePosition = node.transform.position
                });

                onComplete?.Invoke(true);
            }
            else
            {
                onComplete?.Invoke(false);
            }
        }
    }
}
