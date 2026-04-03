using System;
using System.Collections.Generic;
using UnityEngine;
using Dylanng.Core.Base;

namespace Dylanng.Data
{
    public enum SoundType
    {
        Music_MainTheme = 0,
        
        Gameplay_IceBreak = 10,
        Gameplay_Pop = 11,
        Gameplay_Merge = 12,
        Gameplay_Booster_Hammer = 13,
        Gameplay_Booster_Bomb = 14,
        Gameplay_Booster_RandomRocket = 15,
        Gameplay_Booster_Reverse = 16,
        
        UI_Click = 20,
        UI_PopupOpen = 21,
        
        Gameplay_Win,
        Gameplay_Lose,
    }
    
    [Serializable]
    public struct SoundData
    {
        public SoundType Type;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume;
        [Range(-3f, 3f)] public float Pitch;
        
        public SoundData(SoundType type)
        {
            Type = type;
            Clip = null;
            Volume = 1.0f;
            Pitch = 1.0f;
        }
    }

    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "JellySort/Sound Library")]
    public class SoundLibrarySO : ScriptableData
    {
        public List<SoundData> Sounds = new List<SoundData>();
        
        private Dictionary<SoundType, SoundData> _soundDict;

        public void Initialize()
        {
            _soundDict = new Dictionary<SoundType, SoundData>();
            foreach (var sound in Sounds)
            {
                if (!_soundDict.ContainsKey(sound.Type))
                {
                    _soundDict.Add(sound.Type, sound);
                }
            }
        }

        public bool TryGetSound(SoundType type, out SoundData data)
        {
            if (_soundDict == null) Initialize();
            return _soundDict.TryGetValue(type, out data);
        }
    }
}