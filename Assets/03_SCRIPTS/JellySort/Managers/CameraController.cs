using JellySort.GameInputs;
using JellySort.Gameplay.HexaStack;
using UnityEngine;

namespace JellySort.Managers
{
    public class CameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float rotationSpeed = 0.2f;
        [SerializeField] private bool invertRotation = false;
        
        private Vector3 _lastMousePosition;

        private void Update()
        {
            if (TouchInputService.IsDragging || HexaStackController.IsProcessingMerge) return;
            
            float deltaX = 0f;
            bool isInputActive = false;
            
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                
                if (touch.phase == TouchPhase.Moved)
                {
                    deltaX = touch.deltaPosition.x;
                    isInputActive = true;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _lastMousePosition = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    deltaX = Input.mousePosition.x - _lastMousePosition.x;
                    _lastMousePosition = Input.mousePosition;
                    isInputActive = true;
                }
            }
            
            if (isInputActive && Mathf.Abs(deltaX) > 0.01f)
            {
                float direction = invertRotation ? 1f : -1f;
                transform.Rotate(Vector3.up, -deltaX * rotationSpeed * direction * Time.deltaTime, Space.World);
            }
        }
    }
}