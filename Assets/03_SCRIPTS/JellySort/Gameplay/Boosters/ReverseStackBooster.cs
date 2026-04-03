using System.Collections.Generic;
using UnityEngine;
using JellySort.Data;
using DG.Tweening;
using Dylanng.Core;
using Dylanng.Data;
using Dylanng.Managers;
using JellySort.Events;
using JellySort.Gameplay.HexaStack;

namespace JellySort.Gameplay.Boosters
{
    [CreateAssetMenu(fileName = "ReverseStackBooster", menuName = "JellySort/Boosters/Reverse Stack Booster")]
    public class ReverseStackBooster : BaseBooster
    {
        private void OnEnable()
        {
            BoosterType = BoosterType.ReverseStack;
            RequiresTargetSelection = true;
        }

        public override void Execute(BoosterContext context, System.Action onComplete)
        {
            if (context.TargetNode == null || context.TargetNode.StackCount < 2)
            {
                onComplete?.Invoke();
                return;
            }
            
            context.TargetNode.Reverse();
            List<HexaItem> items = context.TargetNode.GetItems();

            Sequence seq = DOTween.Sequence();
            
            float heightStep = 0.23f;
            Vector3 basePosition = context.TargetNode.transform.position;

            for (int i = 0; i < items.Count; i++)
            {
                float newYOffset = (i + 1) * heightStep;
                Vector3 newPos = basePosition + new Vector3(0, newYOffset, 0);
                seq.Join(items[i].transform.DOJump(newPos, 0.5f, 1, 0.5f));
            }
            
            seq.OnComplete(() =>
            {
                EventBus.Publish(new ForceMergeCheckEvent
                {
                    
                    TargetNode = context.TargetNode
                });
                onComplete?.Invoke();
            });
        }
    }
}
