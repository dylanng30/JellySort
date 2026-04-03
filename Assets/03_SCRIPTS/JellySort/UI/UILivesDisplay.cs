using Dylanng.Core;
using JellySort.Events;
using JellySort.Managers;
using TMPro;
using UnityEngine;

namespace JellySort.UI
{
    public class UILivesDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _livesText;

        private void OnEnable()
        {
            EventBus.Subscribe<LivesChangedEvent>(OnLivesChanged);
            RefreshDisplay();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LivesChangedEvent>(OnLivesChanged);
        }
        
        private void RefreshDisplay()
        {
            var livesManager = ServiceLocator.Get<LivesManager>();
            if (livesManager != null)
            {
                if (livesManager.CurrentLives == LivesManager.MAX_LIVES)
                    UpdateDisplay("Full");
                else
                    UpdateDisplay($"0{livesManager.CurrentLives.ToString()}");
            }
        }

        private void OnLivesChanged(LivesChangedEvent evt)
        {
            if(evt.CurrentLives == evt.MaxLives) 
                UpdateDisplay("Full");
            else 
                UpdateDisplay($"0{evt.CurrentLives.ToString()}");
        }
        
        private void UpdateDisplay(string information)
        {
            if (_livesText) _livesText.text = information;
        }
    }
}