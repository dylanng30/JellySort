using Dylanng.Core;
using Dylanng.Core.Base.Interfaces;
using Dylanng.Core.Systems;
using JellySort.Managers;
using UnityEngine;
using TMPro;

namespace JellySort.Gameplay.Grid
{
    public class StackCounter : MonoBehaviour, ILateTickable
    {
        [SerializeField] private TextMeshPro _textLabel;

        private Transform _cameraTransform;
        private void Awake()
        {
            if (_textLabel == null)
                _textLabel = GetComponentInChildren<TextMeshPro>();
        }
        private void OnEnable()
        {
            var updateSystem = ServiceLocator.Get<UpdateSystem>();
            if (updateSystem != null)
            {
                updateSystem.RegisterLate(this); 
            }
        }

        private void OnDisable()
        {
            var updateSystem = ServiceLocator.Get<UpdateSystem>();
            if (updateSystem != null)
            {
                updateSystem.UnregisterLate(this);
            }
        }
        public void LateTick(float deltaTime)
        {
            if (_cameraTransform == null)
            {
                if (Camera.main != null) _cameraTransform = Camera.main.transform;
                else return;
            }
            
            transform.rotation = _cameraTransform.rotation;
        }

        public void SetCount(int count)
        {
            if (count <= 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            if (_textLabel != null)
            {
                _textLabel.text = count.ToString();
            }
        }
    }
}
