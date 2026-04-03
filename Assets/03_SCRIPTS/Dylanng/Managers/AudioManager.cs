using UnityEngine;
using Dylanng.Core;
using Dylanng.Core.Base;
using Dylanng.Data;
using JellySort.Events;
using JellySort.Managers;

namespace Dylanng.Managers
{
    public class AudioManager : ManagerBase
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        
        [Header("Library")]
        [SerializeField] private SoundLibrarySO _soundLibrary;

        public override void Initialize()
        {
            ServiceLocator.Register<AudioManager>(this);

            var saveData = ServiceLocator.Get<SaveLoadManager>()?.Data;
            if (saveData != null)
            {
                SetMusicMute(!saveData.IsMusicOn);
                SetSFXMute(!saveData.IsSoundOn);
            }
            
            PlayMusic(SoundType.Music_MainTheme);
            
            EventBus.Subscribe<IceBrokenEvent>(OnIceBroken);
            EventBus.Subscribe<HexaMergeCompletedEvent>(OnHexaMerged);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.Unsubscribe<IceBrokenEvent>(OnIceBroken);
            EventBus.Unsubscribe<HexaMergeCompletedEvent>(OnHexaMerged);
            ServiceLocator.Unregister<AudioManager>();
        }

        public void PlayMusic(SoundType type, bool loop = true)
        {
            var saveData = ServiceLocator.Get<SaveLoadManager>()?.Data;
            if (saveData != null && !saveData.IsMusicOn) return;

            if (_soundLibrary != null && _soundLibrary.TryGetSound(type, out var soundData))
            {
                if (soundData.Clip == null || _musicSource.clip == soundData.Clip) return;

                _musicSource.clip = soundData.Clip;
                float pitch = soundData.Pitch == 0f ? 1f : soundData.Pitch;
                float volume = soundData.Volume == 0f ? 1f : soundData.Volume;
                _musicSource.volume = volume;
                _musicSource.pitch = pitch;
                _musicSource.loop = loop;
                _musicSource.Play();
            }
            else
            {
                GameLogger.LogWarning($"AudioManager: SoundType {type} chưa được thiết lập trong thư viện!");
            }
        }

        public void PlaySFX(SoundType type)
        {
            var saveData = ServiceLocator.Get<SaveLoadManager>()?.Data;
            if (saveData != null && !saveData.IsSoundOn) return;

            if (_soundLibrary != null && _soundLibrary.TryGetSound(type, out var soundData))
            {
                if (soundData.Clip == null) return;
                float pitch = soundData.Pitch == 0f ? 1f : soundData.Pitch;
                _sfxSource.pitch = pitch;
                _sfxSource.PlayOneShot(soundData.Clip, soundData.Volume);
            }
            else
            {
                GameLogger.LogWarning($"AudioManager: SoundType {type} chưa được thiết lập trong thư viện!");
            }
        }

        public void SetMusicMute(bool mute)
        {
            _musicSource.mute = mute;
        }

        public void SetSFXMute(bool mute)
        {
            _sfxSource.mute = mute;
        }

        public void SetMasterVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            _musicSource.volume = volume;
            _sfxSource.volume = volume;
        }

        #region ---HANDLERS---

        private void OnIceBroken(IceBrokenEvent evt)
        {
            PlaySFX(SoundType.Gameplay_IceBreak);
        }

        private void OnHexaMerged(HexaMergeCompletedEvent evt)
        {
            PlaySFX(SoundType.Gameplay_Merge);
        }

        #endregion
    }
}