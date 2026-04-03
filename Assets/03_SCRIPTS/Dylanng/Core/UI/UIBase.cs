using Dylanng.Core.Base;
using UnityEngine;

namespace Dylanng.Core.UI
{
    public abstract class UIBase : MonoBehaviourBase
    {
        public bool IsOpen { get; private set; }
        public virtual void Initialize() { }
        public virtual void Setup(object data = null) { }
        public virtual void Show()
        {
            IsOpen = true;
            gameObject.SetActive(true);
            OnShow();
        }
        public virtual void Hide()
        {
            IsOpen = false;
            OnHide();
            gameObject.SetActive(false);
        }

        // Hook cho animation hoặc reset state
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }

    
    
}
