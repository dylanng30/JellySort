using Dylanng.Core;
using Dylanng.Core.Base.Interfaces;
using JellySort.Events;
using JellySort.Gameplay.Grid;
using JellySort.Gameplay.HexaStack;
using UnityEngine;

namespace JellySort.GameInputs
{
    public class TouchInputService : MonoBehaviour, IService
    {
        [SerializeField] private LayerMask _draggableLayerMask;
        [SerializeField] private LayerMask _nodeLayerMask;
        
        private Camera _mainCamera;
        private DraggableStack _currentDraggingStack;
        private Plane _dragPlane;
        private Vector3 _dragOffset;
        private HexaNode _hoveredNode;

        public static bool IsDragging;

        public void Initialize()
        {
            _mainCamera = Camera.main;
            ServiceLocator.Register<TouchInputService>(this);
            EventBus.Subscribe<BoosterTargetModeStateChangedEvent>(OnBoosterTargetModeChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<BoosterTargetModeStateChangedEvent>(OnBoosterTargetModeChanged);
            ServiceLocator.Unregister<TouchInputService>();
        }

        private bool _isBoosterTargetMode = false;

        private void OnBoosterTargetModeChanged(BoosterTargetModeStateChangedEvent evt)
        {
            _isBoosterTargetMode = evt.IsActive;
            
            if (_isBoosterTargetMode && _currentDraggingStack != null)
            {
                _currentDraggingStack.ReturnToHome();
                SetDraggingState(false);
            }
        }

        private void SetDraggingState(bool state)
        {
            IsDragging = state;
            if (!state)
            {
                _currentDraggingStack = null;
                if (_hoveredNode != null)
                {
                    _hoveredNode.ShowTarget(false);
                    _hoveredNode = null;
                }
            }
        }

        private void Update()
        {
            if (HexaStackController.IsProcessingMerge) return;
            
            if (_isBoosterTargetMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    HandleBoosterClick(Input.mousePosition);
                }
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartDragging(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                UpdateDragging(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDragging(Input.mousePosition);
            }
        }

        private void HandleBoosterClick(Vector3 screenPos)
        {
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _nodeLayerMask))
            {
                HexaNode node = hit.collider.GetComponentInParent<HexaNode>();
                if (node != null)
                {
                    EventBus.Publish(new TargetNodeSelectedForBoosterEvent { Node = node });
                }
            }
            else 
            {
                EventBus.Publish(new BoosterCanceledEvent());
            }
        }

        private void StartDragging(Vector3 screenPos)
        {
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _draggableLayerMask))
            {
                _currentDraggingStack = hit.collider.GetComponentInParent<DraggableStack>();
                if (_currentDraggingStack != null)
                {
                    IsDragging = true;
                    _currentDraggingStack.SetDragging(true);
                    
                    _dragPlane = new Plane(Vector3.up, _currentDraggingStack.transform.position);
                    
                    if (_dragPlane.Raycast(ray, out float enter))
                    {
                        _dragOffset = _currentDraggingStack.transform.position - ray.GetPoint(enter);
                    }
                    
                    EventBus.Publish(new StackBeginDragEvent { Stack = _currentDraggingStack });
                }
            }
        }

        private void UpdateDragging(Vector3 screenPos)
        {
            if (_currentDraggingStack == null) return;

            Ray ray = _mainCamera.ScreenPointToRay(screenPos);
            Vector3 targetPos = Vector3.zero;
            Quaternion targetRot = Quaternion.identity;
            bool overValidNode = false;
            
            HexaNode currentHoveredNode = null;
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _nodeLayerMask))
            {
                HexaNode node = hit.collider.GetComponentInParent<HexaNode>();
                if (node != null && node.StackCount == 0 && !node.IsLocked)
                {
                    targetPos = node.transform.position + Vector3.up * 0.5f;
                    targetRot =  node.transform.rotation;
                    overValidNode = true;
                    currentHoveredNode = node;
                }
            }
            
            if (currentHoveredNode != _hoveredNode)
            {
                if (_hoveredNode != null) _hoveredNode.ShowTarget(false);
                
                _hoveredNode = currentHoveredNode;
                
                if (_hoveredNode != null) _hoveredNode.ShowTarget(true);
            }
            
            if (!overValidNode)
            {
                if (_dragPlane.Raycast(ray, out float enter))
                {
                    targetPos = ray.GetPoint(enter) + _dragOffset + Vector3.up * 0.25f;
                }
                else return;
            }
            
            _currentDraggingStack.transform.position = Vector3.Lerp(
                _currentDraggingStack.transform.position, 
                targetPos, 
                Time.deltaTime * 25f);
            _currentDraggingStack.transform.rotation = targetRot;
        }

        private void EndDragging(Vector3 screenPos)
        {
            if (_hoveredNode != null)
            {
                _hoveredNode.ShowTarget(false);
                _hoveredNode = null;
            }
            
            if (_currentDraggingStack == null) return;

            HexaNode targetNode = null;
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _nodeLayerMask))
            {
                targetNode = hit.collider.GetComponentInParent<HexaNode>();
            }

            if (targetNode != null && targetNode.StackCount == 0 && !targetNode.IsLocked)
            {
                EventBus.Publish(new StackEndDragEvent 
                { 
                    Stack = _currentDraggingStack, 
                    TargetNode = targetNode 
                });
            }
            else
            {
                _currentDraggingStack.ReturnToHome();
                EventBus.Publish(new StackEndDragEvent 
                { 
                    Stack = _currentDraggingStack, 
                    TargetNode = null 
                });
            }
            
            SetDraggingState(false);
        }
    }
}
