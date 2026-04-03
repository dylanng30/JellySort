using UnityEngine;
using JellySort.Data;
using DG.Tweening;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using Dylanng.Data;
using Dylanng.Managers;
using JellySort.Events;

namespace JellySort.Gameplay.Boosters
{
    [CreateAssetMenu(fileName = "HammerBooster", menuName = "JellySort/Boosters/Hammer Booster")]
    public class HammerBooster : BaseBooster
    {
        public GameObject HammerModelPrefab;
        
        private void OnEnable()
        {
            BoosterType = BoosterType.Hammer;
            RequiresTargetSelection = true;
        }

        public override void Execute(BoosterContext context, System.Action onComplete)
        {
            if (context.TargetNode == null || context.TargetNode.StackCount == 0)
            {
                onComplete?.Invoke();
                return;
            }
            
            Vector3 targetPos = context.TargetNode.GetTopPlacementPosition();
            
            if (HammerModelPrefab != null)
            {
                Vector3 startPos = targetPos + new Vector3(1.5f, 2f, 0f);
                //TODO: Triển kahi Object Pooling
                GameObject hammerObj = Instantiate(HammerModelPrefab, startPos, Quaternion.Euler(0, 0, -45f));

                Sequence swingSeq = DOTween.Sequence();
                
                swingSeq.Append(hammerObj.transform.DORotate(new Vector3(0, 0, 45f), 0.25f).SetEase(Ease.InBack));
                swingSeq.Join(hammerObj.transform.DOMove(targetPos + new Vector3(0.5f, 0.5f, 0f), 0.25f).SetEase(Ease.InBack));
                
                swingSeq.AppendCallback(() =>
                {
                    OnHammerImpact(context, targetPos, onComplete);
                });
                
                swingSeq.Append(hammerObj.transform.DOMoveY(targetPos.y + 1f, 0.2f).SetEase(Ease.OutQuad));
                swingSeq.Join(hammerObj.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
                
                swingSeq.OnComplete(() =>
                {
                    //TODO: Triển kahi Object Pooling
                    Destroy(hammerObj);
                });
            }
            else
            {
                OnHammerImpact(context, targetPos, onComplete);
            }
        }

        private void OnHammerImpact(BoosterContext context, Vector3 targetPos, System.Action onComplete)
        {
            ServiceLocator.Get<AudioManager>()?.PlaySFX(SoundType.Gameplay_Booster_Hammer);
            
            if (UseVfxPrefab != null)
            {
                ParticleSystem vfx = Instantiate(UseVfxPrefab, targetPos, Quaternion.identity);
                vfx.Play();
                Destroy(vfx.gameObject, vfx.main.duration + vfx.main.startLifetime.constantMax);
            }
            
            context.TargetNode.transform.DOShakeScale(0.2f, 0.25f, 10, 90f);
            
            if (context.TargetNode.IsIceGrid)
            {
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    context.TargetNode.transform.localScale = Vector3.one;
                    if (context.TargetNode.DamageIce())
                    {
                        EventBus.Publish(new IceBrokenEvent { Node = context.TargetNode, Position = context.TargetNode.transform.position });
                    }
                    onComplete?.Invoke();
                });
                return;
            }
            
            var items = context.TargetNode.GetItems();
            Sequence popSeq = DOTween.Sequence();
            
            foreach (var item in items)
            {
                popSeq.Join(item.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
            }

            popSeq.OnComplete(() => 
            {
                var poolManager = ServiceLocator.Get<PoolManager>();
                foreach (var item in items)
                {
                    item.transform.localScale = Vector3.one;
                    poolManager.Despawn("HexaItem", item);
                }
                context.TargetNode.ClearStack();
                
                EventBus.Publish(new ForceMergeCheckEvent
                {
                    TargetNode = context.TargetNode
                });
                
                onComplete?.Invoke();
            });
        }
        
    }
}
