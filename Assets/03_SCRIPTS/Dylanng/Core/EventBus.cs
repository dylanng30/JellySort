using System;
using System.Collections.Generic;
using Dylanng.Core.Base.Interfaces;

namespace Dylanng.Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> subs = new ();

        public static void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            Type t = typeof(T);
            if (!subs.TryGetValue(t, out List<Delegate> list))
            {
                list = new List<Delegate>();
                subs[t] = list;
            }
            list.Add(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            Type t = typeof(T);
            if (subs.TryGetValue(t, out List<Delegate> list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                    subs.Remove(t);
            }
        }


        public static void Publish<T>(T eventData) where T : IEvent
        {
            Type t = typeof(T);
            if (subs.TryGetValue(t, out List<Delegate> list))
            {
                Delegate[] delegates = list.ToArray();
                foreach (Delegate d in delegates)
                {
                    ((Action<T>)d)(eventData);
                }
            }
        }
        
    }
}
