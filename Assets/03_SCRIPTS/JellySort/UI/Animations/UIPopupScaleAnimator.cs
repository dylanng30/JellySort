using UnityEngine;
using DG.Tweening;

namespace JellySort.UI.Animations
{
    public class UIPopupScaleAnimator : UIAnimationBase
    {
        [Header("Popup Animation Settings")]
        [SerializeField] private float _animationDuration = 0.35f;
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField] private Vector3 _targetScale = Vector3.one;

        protected override void OnEnable()
        {
            ResetToInitialState();
            PlayShowAnimation();
        }

        public override void ResetToInitialState()
        {
            transform.localScale = Vector3.zero;
        }

        public void PlayShowAnimation()
        {
            KillCurrentTween();
            _currentTween = transform.DOScale(_targetScale, _animationDuration)
                .SetEase(_showEase)
                .SetUpdate(true);
        }
    }
}