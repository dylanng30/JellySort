using System;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using Dylanng.Core.Systems;
using Dylanng.Core.UI;
using JellySort.GameInputs;
using JellySort.Managers;
using UnityEngine;

namespace Dylanng.Managers
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("Managers")]
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private PoolManager _poolManager;
        [SerializeField] private SaveLoadManager _saveLoadManager;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private GameplayStateController _gameplayStateController;
        [SerializeField] private TouchInputService _touchInputService;
        [SerializeField] private BoosterManager _boosterManager;
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private EconomyManager _economyManager;
        [SerializeField] private LivesManager _livesManager;
        [SerializeField] private ComboManager _comboManager;
        [SerializeField] private VFXManager _vfxManager;
        
        private UpdateSystem _updateSystem;

        protected override void Awake()
        {
            base.Awake();
            
            Initialize();
        }

        private void OnDestroy()
        {
            if(_updateSystem != null)
                _updateSystem.OnDestroy();
        }

        private void Initialize()
        {
            _updateSystem = new UpdateSystem();
            _updateSystem.Initialize();
            
            _saveLoadManager.Initialize();
            _poolManager.Initialize();
            _vfxManager.Initialize();
            _levelManager.Initialize();
            _boosterManager.Initialize();
            _audioManager.Initialize();
            _economyManager.Initialize();
            _livesManager.Initialize();
            _comboManager.Initialize();
            _touchInputService.Initialize();
            _uiManager.Initialize();
            _gameplayStateController.Initialize();
        }
        
        private void Update()
        {
            _updateSystem?.Tick(Time.deltaTime);
        }

        private void LateUpdate()
        {
            _updateSystem?.LateTick(Time.deltaTime);
        }
    }
}
