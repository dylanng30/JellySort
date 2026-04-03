using System.Collections.Generic;
using JellySort.Gameplay.HexaStack;
using UnityEngine;

namespace JellySort.Gameplay.Grid
{
    public class HexaNode : MonoBehaviour
    {
        public HexaCoordinates Coordinates { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsIceGrid { get; private set; }
        
        public int StackCount => _itemsStack.Count;
        public List<HexaItem> GetItems() => _itemsStack;
        
        [SerializeField] private StackCounter _stackCounter;
        [SerializeField] private GameObject _targetSign;
        [SerializeField] private GameObject _iceVisual;
        
        private List<HexaItem> _itemsStack = new List<HexaItem>();
        private int _iceHP;
        
        public void Setup(HexaCoordinates coords, bool isLocked, bool isIceGrid)
        {
            Coordinates = coords;
            IsIceGrid = isIceGrid;
            
            if (IsIceGrid)
            {
                IsLocked = true;
                _iceHP = 1;
                if (_iceVisual != null)
                {
                    _iceVisual.SetActive(true);
                }
            }
            else
            {
                IsLocked = isLocked;
                if (_iceVisual != null) _iceVisual.SetActive(false);
            }
            
            _itemsStack.Clear();
            
            gameObject.name = $"Node_{coords.Q}_{coords.R}_{coords.S}";
            UpdateStackCounter();
        }
        
        public void ShowTarget(bool isShow)
        {
            if (_targetSign != null)
            {
                _targetSign.SetActive(isShow);
            }
        }
        
        public bool DamageIce()
        {
            if (!IsIceGrid) return false;

            _iceHP--;
            if (_iceHP <= 0)
            {
                IsIceGrid = false;
                IsLocked = false;
                if (_iceVisual != null) _iceVisual.SetActive(false);
                return true;
            }
            return false;
        }

        public void AddToStack(HexaItem item)
        {
            _itemsStack.Add(item);
            item.transform.SetParent(this.transform);
            if (IsIceGrid)
            {
                _iceVisual.transform.localScale = new Vector3(1.05f, StackCount + 1, 1.05f);
            }
            UpdateStackCounter();
        }

        public void RemoveFromStack(HexaItem item)
        {
            _itemsStack.Remove(item);
            UpdateStackCounter();
        }

        public void Reverse()
        {
            _itemsStack.Reverse();
            UpdateStackCounter();
        }

        public HexaItem GetTopItem()
        {
            if (_itemsStack.Count > 0)
                return _itemsStack[^1];
            return null;
        }

        public Vector3 GetTopPlacementPosition()
        {
            float yOffset = (_itemsStack.Count + 1) * 0.25f; 
            return transform.position + new Vector3(0, yOffset, 0);
        }

        public void ClearStack()
        {
            _itemsStack.Clear();
            UpdateStackCounter();
        }

        private void UpdateStackCounter()
        {
            if (_stackCounter == null) return;

            if (_itemsStack.Count == 0)
            {
                _stackCounter.SetCount(0);
                return;
            }
            
            var topColor = _itemsStack[^1].ColorType;
            int count = 0;
            
            for (int i = _itemsStack.Count - 1; i >= 0; i--)
            {
                if (_itemsStack[i].ColorType == topColor)
                    count++;
                else
                    break;
            }

            _stackCounter.SetCount(count);
            
            float yOffset = (_itemsStack.Count + 1) * 0.25f;
            _stackCounter.transform.position = transform.position + new Vector3(0, yOffset, 0);
        }
    }
}
