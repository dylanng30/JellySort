using Dylanng.Core;
using Dylanng.Core.Base;
using JellySort.Data;
using JellySort.Events;
using UnityEngine;

namespace JellySort.Managers
{
    public class SaveLoadManager : ManagerBase
    {
        private const string SAVE_KEY = "JellySort_SaveData";
        private SaveData _data;

        public SaveData Data => _data;

        public override void Initialize()
        {
            ServiceLocator.Register<SaveLoadManager>(this);
            Load();
            
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
            ServiceLocator.Unregister<SaveLoadManager>();
        }

        public void Save()
        {
            if (_data == null) return;
            string json = JsonUtility.ToJson(_data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
            GameLogger.Log("Game Saved: " + json);
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                _data = JsonUtility.FromJson<SaveData>(json);
                GameLogger.Log("Game Loaded: Level " + _data.CurrentLevel);
            }
            else
            {
                _data = new SaveData();
                Save();
                GameLogger.Log("Initial Save Created.");
            }
        }

        private void OnLevelCompleted(LevelCompletedEvent evt)
        {
            _data.CurrentLevel++;
            Save();
        }

        public int GetBoosterCount(BoosterType type)
        {
            var booster = _data.Boosters.Find(b => b.Type == type);
            return booster.Count;
        }

        public void UpdateBoosterCount(BoosterType type, int amount)
        {
            int index = _data.Boosters.FindIndex(b => b.Type == type);
            if (index != -1)
            {
                var b = _data.Boosters[index];
                b.Count += amount;
                _data.Boosters[index] = b;
                Save();
            }
        }

        public void ToggleSound(bool isOn)
        {
            _data.IsSoundOn = isOn;
            Save();
        }

        public void ToggleMusic(bool isOn)
        {
            _data.IsMusicOn = isOn;
            Save();
        }

        public void ToggleHaptic(bool isOn)
        {
            _data.IsHapticOn = isOn;
            Save();
        }
    }
}
