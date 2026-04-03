using System;
using Dylanng.Core;
using Dylanng.Data;
using Dylanng.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace JellySort.Helpers
{
    [RequireComponent(typeof(Button))]
    public class ButtonSoundPlayer : MonoBehaviour
    {
        [SerializeField] private SoundType soundType = SoundType.UI_Click;
        [SerializeField] private Button _button;
        
        private void Awake()
        {
            if(_button == null) _button = GetComponent<Button>();
            if(_button != null) _button.onClick.AddListener(PlaySound);
        }

        private void OnDestroy()
        {
            if(_button != null) _button.onClick.RemoveListener(PlaySound);
        }

        private void PlaySound()
        {
            Debug.Log("Playing sound");
            ServiceLocator.Get<AudioManager>()?.PlaySFX(soundType);
        }
    }
}