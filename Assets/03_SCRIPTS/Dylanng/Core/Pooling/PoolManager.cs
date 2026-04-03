using System.Collections.Generic;
using Dylanng.Core.Base;
using Unity.VisualScripting;
using UnityEngine;

namespace Dylanng.Core.Pooling
{
    public class PoolManager : ManagerBase
    {
        private Dictionary<string, Queue<PoolableObject>> _poolDictionary = new Dictionary<string, Queue<PoolableObject>>();
        private Dictionary<string, PoolableObject> _prefabs = new Dictionary<string, PoolableObject>();
        private Transform _poolRoot;

        public override void Initialize()
        {
            _poolRoot = new GameObject("[Pool]").transform;
            DontDestroyOnLoad(_poolRoot.gameObject);
            ServiceLocator.Register<PoolManager>(this);
        }

        public void CreatePool(string poolKey, PoolableObject prefab, int initialSize)
        {
            if (!_poolDictionary.ContainsKey(poolKey))
            {
                _poolDictionary.Add(poolKey, new Queue<PoolableObject>());
                _prefabs.Add(poolKey, prefab);

                for (int i = 0; i < initialSize; i++)
                {
                    CreateNewObject(poolKey, prefab);
                }
            }
        }

        private PoolableObject CreateNewObject(string poolKey, PoolableObject prefab)
        {
            var obj = Instantiate(prefab, _poolRoot);
            obj.gameObject.SetActive(false);
            _poolDictionary[poolKey].Enqueue(obj);
            return obj;
        }

        public T Spawn<T>(string poolKey, Vector3 position, Quaternion rotation) where T : PoolableObject
        {
            if (!_poolDictionary.ContainsKey(poolKey)) return null;

            if (_poolDictionary[poolKey].Count == 0)
            {
                CreateNewObject(poolKey, _prefabs[poolKey]);
            }

            var obj = _poolDictionary[poolKey].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.OnSpawn();
            return obj as T;
        }

        public void Despawn(string poolKey, PoolableObject obj)
        {
            obj.OnDespawn();
            obj.transform.SetParent(_poolRoot);
            _poolDictionary[poolKey].Enqueue(obj);
        }

        protected override void OnDestroy()
        {
            ServiceLocator.Unregister<PoolManager>();
        }
    }
}
