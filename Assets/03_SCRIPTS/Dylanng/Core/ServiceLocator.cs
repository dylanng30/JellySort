using System;
using System.Collections.Generic;
using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

        public static void Register<T>(T service) where T : IService
        {
            var type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                _services.Add(type, service);
                GameLogger.Log($"Registered Service: {type.Name}");
            }
            else
            {
                GameLogger.LogWarning($"Service {type.Name} is already registered.");
            }
        }

        public static void Unregister<T>() where T : IService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type)) _services.Remove(type);
        }

        public static T Get<T>() where T : IService
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            GameLogger.LogWarning($"Service {type.Name} not found. Are you sure it's registered?");
            return default;
        }
    }
}
