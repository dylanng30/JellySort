using System.Collections.Generic;
using DG.Tweening;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using TMPro;
using UnityEngine;

namespace JellySort.UI
{
    public class UIFloatingText : PoolableObject
    {
        [SerializeField] private TextMeshProUGUI _textMesh; 
        [SerializeField] private RectTransform rect;
        [SerializeField] private List<Color> comboColors;
        
        public RectTransform Rect => rect;
        public List<Color> ComboColors => comboColors;
        
        [Header("Animation Settings")]
        [SerializeField] private float _floatDistance = 100f;
        [SerializeField] private float _floatDuration = 1.0f;
        [SerializeField] private float _bounceDuration = 0.3f;
        [SerializeField] private float _bounceScale = 1.3f;

        protected override void Awake()
        {
            base.Awake();
            if (_textMesh == null) _textMesh = GetComponentInChildren<TextMeshProUGUI>();
            if (rect == null) rect = GetComponent<RectTransform>();
        }

        public void Setup(string text, Color color)
        {
            rect.DOKill();
            _textMesh.DOKill();

            _textMesh.text = text;
            _textMesh.color = color;
            _textMesh.alpha = 1f;
            rect.localScale = Vector3.one;
            
            Sequence bounceSeq = DOTween.Sequence();
            bounceSeq.Append(rect.DOScale(_bounceScale, _bounceDuration / 2f).SetEase(Ease.OutBack));
            bounceSeq.Append(rect.DOScale(1f, _bounceDuration / 2f).SetEase(Ease.InBack));
            
            Sequence floatSeq = DOTween.Sequence();
            floatSeq.Join(rect.DOAnchorPosY(rect.anchoredPosition.y + _floatDistance, _floatDuration).SetEase(Ease.OutQuad));
            floatSeq.Join(_textMesh.DOFade(0f, _floatDuration).SetEase(Ease.InQuad));
            
            Sequence mainSeq = DOTween.Sequence();
            mainSeq.Insert(0, bounceSeq);
            mainSeq.Insert(0, floatSeq);
            mainSeq.OnComplete(() =>
            {
                ServiceLocator.Get<PoolManager>().Despawn("UIFloatingText", this);
            });
        }

        public override void OnDespawn()
        {
            base.OnDespawn();

            rect.DOKill();
            _textMesh.DOKill();
        }
    }
}