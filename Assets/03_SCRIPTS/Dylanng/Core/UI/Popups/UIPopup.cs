using UnityEngine;

namespace Dylanng.Core.UI.Popups
{
    public abstract class UIPopup : UIBase
    {
        [Header("Popup Settings")]
        [SerializeField] protected bool _closeOnBackdropClick = true;

        public virtual void Close()
        {
            ServiceLocator.Get<UIManager>().CloseCurrentPopup();
        }
    }
}