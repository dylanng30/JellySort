using Dylanng.Core.Pooling;
using JellySort.Data;
using UnityEngine;
using DG.Tweening;
using Dylanng.Core;
using JellySort.Managers;

namespace JellySort.Gameplay.HexaStack
{
    public class HexaItem : PoolableObject
    {
        public HexaColor ColorType { get; private set; }
        
        [SerializeField] private MeshRenderer _meshRenderer;
        
        public void Setup(HexaColor color)
        {
            ColorType = color;
            _meshRenderer.sharedMaterial = ServiceLocator.Get<LevelManager>().GetMaterialForColor(color);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            gameObject.SetActive(true);
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            transform.DOKill();
        }

        public Tween MoveTo(Vector3 targetPosition, float duration = 0.3f)
        {
            return transform.DOJump(targetPosition, jumpPower: 1f, numJumps: 1, duration)
                .SetEase(Ease.OutQuad);
        }
    }
}
