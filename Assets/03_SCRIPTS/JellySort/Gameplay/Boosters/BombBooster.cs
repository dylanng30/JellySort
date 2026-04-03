using System.Collections.Generic;
using UnityEngine;
using JellySort.Data;
using DG.Tweening;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using Dylanng.Data;
using Dylanng.Managers;
using JellySort.Events;
using JellySort.Gameplay.HexaStack;
using JellySort.Gameplay.Grid;

namespace JellySort.Gameplay.Boosters
{
    [CreateAssetMenu(fileName = "BombBooster", menuName = "JellySort/Boosters/Bomb Booster")]
    public class BombBooster : BaseBooster
    {
        public GameObject BombPrefab;
        public float DropDuration = 0.6f;
        
        private void OnEnable()
        {
            BoosterType = BoosterType.Bomb;
            RequiresTargetSelection = true;
        }

        public override void Execute(BoosterContext context, System.Action onComplete)
        {
            if (context.TargetNode == null)
            {
                onComplete?.Invoke();
                return;
            }
            
            Vector3 targetPos = context.TargetNode.GetTopPlacementPosition();

            List<HexaNode> nodesToClear = new List<HexaNode>();
            nodesToClear.Add(context.TargetNode);
            nodesToClear.AddRange(context.Board.GetNeighbors(context.TargetNode.Coordinates));

            List<HexaItem> allItemsToPop = new List<HexaItem>();
            foreach (var node in nodesToClear)
            {
                if (!node.IsIceGrid)
                {
                    allItemsToPop.AddRange(node.GetItems());
                }
            }
            
            if (BombPrefab != null)
            {
                Vector3 spawnPos = targetPos + new Vector3(0f, 15f, 0f);
                GameObject bombObj = Instantiate(BombPrefab, spawnPos, Quaternion.identity);
                
                bombObj.transform.DORotate(new Vector3(360f, 360f, 0f), DropDuration, RotateMode.FastBeyond360).SetEase(Ease.InQuad);
                
                bombObj.transform.DOMove(targetPos, DropDuration).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    Destroy(bombObj);
                    OnBombImpact(nodesToClear, allItemsToPop, targetPos, onComplete);
                });
            }
            else
            {
                // Fallback nếu quên chưa gán Prefab Model Bom
                OnBombImpact(nodesToClear, allItemsToPop, targetPos, onComplete);
            }
        }

        private void OnBombImpact(List<HexaNode> nodesToClear, List<HexaItem> allItemsToPop, Vector3 targetPos, System.Action onComplete)
        {
            ServiceLocator.Get<AudioManager>()?.PlaySFX(SoundType.Gameplay_Booster_Bomb);
            
            if (Camera.main != null)
            {
                Camera.main.transform.DOShakePosition(0.4f, strength: 0.5f, vibrato: 15, randomness: 90f);
            }
            
            if (UseVfxPrefab != null)
            {
                ParticleSystem vfx = Instantiate(UseVfxPrefab, targetPos, Quaternion.identity);
                vfx.Play();
                Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax);
            }
            
            Sequence popSeq = DOTween.Sequence();
            bool hasIceBroken = false;

            foreach (var node in nodesToClear)
            {
                if (node.IsIceGrid)
                {
                    if (node.DamageIce())
                    {
                        EventBus.Publish(new IceBrokenEvent { Node = node, Position = node.transform.position });
                        hasIceBroken = true;
                    }
                }
                else if (node.StackCount > 0)
                {
                    var items = node.GetItems();
                    foreach (var item in items)
                    {
                        popSeq.Join(item.transform.DOJump(item.transform.position + Random.insideUnitSphere * 0.8f, 1f, 1, 0.3f));
                        popSeq.Join(item.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InQuad));
                    }
                }
            }

            if (allItemsToPop.Count == 0 && !hasIceBroken)
            {
                onComplete?.Invoke();
                return;
            }
            
            popSeq.OnComplete(() => 
            {
                var poolManager = ServiceLocator.Get<PoolManager>();
                foreach (var item in allItemsToPop)
                {
                    item.transform.localScale = Vector3.one;
                    poolManager.Despawn("HexaItem", item);
                }
                
                foreach (var node in nodesToClear)
                {
                    if (!node.IsIceGrid)
                    {
                        node.ClearStack();
                        EventBus.Publish(new ForceMergeCheckEvent
                        {
                            TargetNode = node
                        });
                    }
                }
                
                onComplete?.Invoke();
            });
        }
    }
}
