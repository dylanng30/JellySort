using Dylanng.Core;
using Dylanng.Core.Base;
using JellySort.Events;
using System;
using Unity.VisualScripting;
using UnityEngine;
using EventBus = Dylanng.Core.EventBus;

namespace JellySort.Managers
{
    public class LivesManager : ManagerBase
    {
        public const int MAX_LIVES = 5;
        public const int REGEN_TIME_SECONDS = 120;

        private SaveLoadManager _saveManager;
        private int _currentLives;
        private long _nextLifeRegenTimeTicks;
        private float _timer;

        public int CurrentLives => _currentLives;

        public override void Initialize()
        {
            ServiceLocator.Register<LivesManager>(this);
            _saveManager = ServiceLocator.Get<SaveLoadManager>();
            
            LoadLivesData();
            CheckOfflineRecovery();
            
            EventBus.Subscribe<LevelFailedEvent>(OnLevelFailed);
            
            PublishLivesChanged();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.Unsubscribe<LevelFailedEvent>(OnLevelFailed);
            ServiceLocator.Unregister<LivesManager>();
        }

        private void LoadLivesData()
        {
            _currentLives = _saveManager.Data.Lives;
            _nextLifeRegenTimeTicks = _saveManager.Data.NextLifeRegenTimeTicks;
        }

        private void CheckOfflineRecovery()
        {
            if (_currentLives >= MAX_LIVES)
            {
                _nextLifeRegenTimeTicks = 0;
                SaveLivesData();
                return;
            }

            DateTime now = DateTime.UtcNow;
            DateTime target = new DateTime(_nextLifeRegenTimeTicks);

            if (now >= target)
            {
                long elapsedTicks = now.Ticks - target.Ticks;
                long regenIntervalTicks = TimeSpan.FromSeconds(REGEN_TIME_SECONDS).Ticks;

                int recoveredLives = 1 + (int)(elapsedTicks / regenIntervalTicks);
                _currentLives = Math.Min(MAX_LIVES, _currentLives + recoveredLives);

                if (_currentLives >= MAX_LIVES)
                {
                    _nextLifeRegenTimeTicks = 0;
                }
                else
                {
                    long remainingTicksFromLastRegen = elapsedTicks % regenIntervalTicks;
                    _nextLifeRegenTimeTicks = now.AddTicks(regenIntervalTicks - remainingTicksFromLastRegen).Ticks;
                }
                
                SaveLivesData();
            }
        }

        private void Update()
        {
            if (_currentLives >= MAX_LIVES) return;

            _timer += Time.deltaTime;
            if (_timer >= 1.0f)
            {
                _timer = 0;
                long nowTicks = DateTime.UtcNow.Ticks;
                
                if (nowTicks >= _nextLifeRegenTimeTicks)
                {
                    _currentLives++;
                    if (_currentLives >= MAX_LIVES)
                    {
                        _nextLifeRegenTimeTicks = 0;
                    }
                    else
                    {
                        _nextLifeRegenTimeTicks += TimeSpan.FromSeconds(REGEN_TIME_SECONDS).Ticks;
                    }
                    
                    SaveLivesData();
                    PublishLivesChanged();
                }
                
                PublishTimerTick();
            }
        }

        public bool HasLives()
        {
            return _currentLives > 0;
        }

        public void ConsumeLife()
        {
            if (_currentLives <= 0) return;

            if (_currentLives == MAX_LIVES)
            {
                _nextLifeRegenTimeTicks = DateTime.UtcNow.AddSeconds(REGEN_TIME_SECONDS).Ticks;
            }

            _currentLives--;
            SaveLivesData();
            PublishLivesChanged();
            
            GameLogger.Log($"Life Consumed. Remaining: {_currentLives}");
        }

        private void OnLevelFailed(LevelFailedEvent evt)
        {
            ConsumeLife();
        }

        private void SaveLivesData()
        {
            _saveManager.Data.Lives = _currentLives;
            _saveManager.Data.NextLifeRegenTimeTicks = _nextLifeRegenTimeTicks;
            _saveManager.Save();
        }

        private void PublishLivesChanged()
        {
            EventBus.Publish(new LivesChangedEvent 
            { 
                CurrentLives = _currentLives, 
                MaxLives = MAX_LIVES 
            });
        }

        private void PublishTimerTick()
        {
            if (_nextLifeRegenTimeTicks == 0) return;

            TimeSpan remaining = new DateTime(_nextLifeRegenTimeTicks) - DateTime.UtcNow;
            int seconds = (int)remaining.TotalSeconds;
            
            EventBus.Publish(new LivesRegenTimerTickEvent 
            { 
                SecondsRemaining = Math.Max(0, seconds) 
            });
        }
    }
}
