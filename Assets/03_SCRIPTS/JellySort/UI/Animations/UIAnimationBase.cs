using UnityEngine;
using DG.Tweening;

namespace JellySort.UI.Animations
{
    public abstract class UIAnimationBase : MonoBehaviour
    {
        protected Tween _currentTween;

        protected virtual void OnEnable()
        {
            ResetToInitialState();
        }

        protected virtual void OnDisable()
        {
            KillCurrentTween();
        }

        protected virtual void OnDestroy()
        {
            KillCurrentTween();
        }
        
        protected void KillCurrentTween()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
                _currentTween = null;
            }
        }
        
        public abstract void ResetToInitialState();
    }
}
