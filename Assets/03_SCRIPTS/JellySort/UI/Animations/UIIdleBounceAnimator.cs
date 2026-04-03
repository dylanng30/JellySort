using UnityEngine;
using DG.Tweening;

namespace JellySort.UI.Animations
{
    public class UIIdleBounceAnimator : UIAnimationBase
    {
        [Header("Bounce Settings")]
        [SerializeField] private float _bounceScaleMultipler = 1.05f;
        [SerializeField] private float _cycleDuration = 0.8f;
        [SerializeField] private Ease _easeType = Ease.InOutSine;

        private Vector3 _originalScale;
        private bool _isInitialized;

        private void Awake()
        {
            InitializeScale();
        }

        private void InitializeScale()
        {
            if (!_isInitialized)
            {
                _originalScale = transform.localScale;
                _isInitialized = true;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartBouncing();
        }

        public override void ResetToInitialState()
        {
            InitializeScale();
            transform.localScale = _originalScale;
        }

        private void StartBouncing()
        {
            KillCurrentTween();
            _currentTween = transform.DOScale(_originalScale * _bounceScaleMultipler, _cycleDuration)
                .SetEase(_easeType)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }
    }
}