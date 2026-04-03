using UnityEngine;
using DG.Tweening;

namespace JellySort.UI.Animations
{
    public class UIIdleWobbleAnimator : UIAnimationBase
    {
        [Header("Wobble Settings")]
        [SerializeField] private float _wobbleAngle = 10f;
        [SerializeField] private float _cycleDuration = 0.5f;
        [SerializeField] private Ease _easeType = Ease.InOutQuad;

        private Quaternion _originalRotation;
        private bool _isInitialized;

        private void Awake()
        {
            InitializeRotation();
        }

        private void InitializeRotation()
        {
            if (!_isInitialized)
            {
                _originalRotation = transform.localRotation;
                _isInitialized = true;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartWobbling();
        }

        public override void ResetToInitialState()
        {
            InitializeRotation();
            transform.localRotation = _originalRotation;
        }

        private void StartWobbling()
        {
            KillCurrentTween();
            
            transform.localRotation = _originalRotation * Quaternion.Euler(0, 0, -_wobbleAngle);
            
            _currentTween = transform.DORotate(new Vector3(0, 0, _wobbleAngle), _cycleDuration)
                .SetEase(_easeType)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true)
                .SetUpdate(true);
        }
    }
}