using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JellySort.Data;
using DG.Tweening;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using Dylanng.Data;
using Dylanng.Managers;
using JellySort.Events;
using JellySort.Gameplay.Grid;
using JellySort.Gameplay.HexaStack;

namespace JellySort.Gameplay.Boosters
{
    [CreateAssetMenu(fileName = "RandomRocketBooster", menuName = "JellySort/Boosters/Random Rocket Booster")]
    public class RandomRocketBooster : BaseBooster
    {
        public GameObject RocketPrefab;
        public float FlightDuration = 0.8f;
        public float DelayStep = 0.3f;
        
        private void OnEnable()
        {
            BoosterType = BoosterType.RandomRockets;
            RequiresTargetSelection = false;
        }

        public override void Execute(BoosterContext context, System.Action onComplete)
        {
            var allNodes = context.Board.GetAllNodes()
                .Where(n => n.StackCount > 0 && !n.IsLocked)
                .ToList();
            
            if (allNodes.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }
            
            //Shuffle
            System.Random rnd = new System.Random();
            var shuffledNodes = allNodes.OrderBy(x => rnd.Next()).ToList();
            var targetNodes = shuffledNodes.Take(Mathf.Min(3, shuffledNodes.Count)).ToList();

            Sequence masterSeq = DOTween.Sequence();
            List<HexaItem> allItemsToPop = new List<HexaItem>();
            int completedRockets = 0;

            for (int i = 0; i < targetNodes.Count; i++)
            {
                var node = targetNodes[i];
                float currentDelay = i * DelayStep;
                Vector3 targetPos = node.GetTopPlacementPosition();
                
                if (!node.IsIceGrid)
                {
                    allItemsToPop.AddRange(node.GetItems());
                }
                
                if (RocketPrefab != null)
                {
                    masterSeq.InsertCallback(currentDelay, () => 
                    {
                        Vector3 spawnPos = targetPos + new Vector3(Random.Range(-4f, 4f), 12f, Random.Range(-4f, 4f));
                        GameObject rocketObj = Instantiate(RocketPrefab, spawnPos, Quaternion.identity);
                        
                        rocketObj.transform.LookAt(targetPos);
                        rocketObj.transform.DOMove(targetPos, FlightDuration).SetEase(Ease.InQuad).OnComplete(() =>
                        {
                            Destroy(rocketObj);
                            OnRocketImpact(node, targetPos);
                            
                            completedRockets++;
                            if (completedRockets == targetNodes.Count)
                            {
                                FinishExecution(targetNodes, allItemsToPop, onComplete);
                            }
                        });
                    });
                }
                else
                {
                    masterSeq.InsertCallback(currentDelay, () =>
                    {
                        OnRocketImpact(node, targetPos);
                        completedRockets++;
                        if (completedRockets == targetNodes.Count)
                        {
                            FinishExecution(targetNodes, allItemsToPop, onComplete);
                        }
                    });
                }
            }
        }

        private void OnRocketImpact(HexaNode node, Vector3 targetPos)
        {
            ServiceLocator.Get<AudioManager>()?.PlaySFX(SoundType.Gameplay_Booster_RandomRocket);
            
            if (Camera.main != null)
            {
                Camera.main.transform.DOShakePosition(0.2f, strength: 0.25f, vibrato: 10, randomness: 90f);
            }
            
            if (UseVfxPrefab != null)
            {
                ParticleSystem vfx = Instantiate(UseVfxPrefab, targetPos, Quaternion.identity);
                vfx.Play();
                Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax);
            }
            
            if (node.IsIceGrid)
            {
                if (node.DamageIce())
                {
                    EventBus.Publish(new IceBrokenEvent { Node = node, Position = node.transform.position });
                }
            }
            else
            {
                var items = node.GetItems();
                foreach (var item in items)
                {
                    item.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
                }
            }
        }
        
        private void FinishExecution(List<HexaNode> targetNodes, List<HexaItem> allItemsToPop, System.Action onComplete)
        {
            DOVirtual.DelayedCall(0.25f, () =>
            {
                var poolManager = ServiceLocator.Get<PoolManager>();
                foreach (var item in allItemsToPop)
                {
                    item.transform.localScale = Vector3.one;
                    poolManager.Despawn("HexaItem", item);
                }

                foreach (var node in targetNodes)
                {
                    if (!node.IsIceGrid)
                    {
                        node.ClearStack();
                        EventBus.Publish(new ForceMergeCheckEvent { TargetNode = node });
                    }
                }
                
                onComplete?.Invoke();
            });
        }
    }
}
