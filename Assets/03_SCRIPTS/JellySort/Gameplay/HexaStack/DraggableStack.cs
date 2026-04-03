using System.Collections.Generic;
using DG.Tweening;
using Dylanng.Core;
using Dylanng.Core.Pooling;
using JellySort.Gameplay.Grid;
using UnityEngine;

namespace JellySort.Gameplay.HexaStack
{
    public class DraggableStack : PoolableObject
    {
        private List<HexaItem> _items = new List<HexaItem>();
        private Vector3 _originalPosition;
        private bool _isDragging;
        
        [SerializeField] private StackCounter _stackCounter;
        
        public List<HexaItem> Items => _items;
        public int Count => _items.Count;
        
        public override void OnDespawn()
        {
            base.OnDespawn();
            
            if (_items.Count > 0)
            {
                var poolManager = ServiceLocator.Get<PoolManager>();
                if (poolManager != null)
                {
                    foreach (var item in _items)
                    {
                        item.transform.SetParent(null); 
                        poolManager.Despawn("HexaItem", item);
                    }
                }
                _items.Clear();
                UpdateStackCounter();
            }
        }

        public void Setup(List<HexaItem> items)
        {
            _items = items;
            _originalPosition = transform.localPosition;
            
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].transform.SetParent(this.transform);
                float yOffset = i * 0.25f; 
                _items[i].transform.localPosition = new Vector3(0, yOffset, 0);
            }
            
            UpdateStackCounter();
        }

        public void SetDragging(bool dragging)
        {
            _isDragging = dragging;
            if (_isDragging)
            {
                transform.DOMoveY(_originalPosition.y + 0.25f, 0.1f);
            }
        }

        public void ReturnToHome()
        {
            _isDragging = false;
            transform.DOLocalMove(_originalPosition, 0.3f).SetEase(Ease.OutBack);
        }

        public void Clear()
        {
            _items.Clear();
            UpdateStackCounter();
        }

        public HexaItem PopItem()
        {
            if (_items.Count == 0) return null;
            var item = _items[^1];
            _items.RemoveAt(_items.Count - 1);
            UpdateStackCounter();
            return item;
        }

        private void UpdateStackCounter()
        {
            if (_stackCounter == null) return;

            if (_items.Count == 0)
            {
                _stackCounter.SetCount(0);
                return;
            }

            var topColor = _items[_items.Count - 1].ColorType;
            int count = 0;

            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i].ColorType == topColor) count++;
                else break;
            }

            _stackCounter.SetCount(count);

            float yOffset = _items.Count * 0.25f;
            _stackCounter.transform.localPosition = new Vector3(0, yOffset, 0);
        }
    }
}

