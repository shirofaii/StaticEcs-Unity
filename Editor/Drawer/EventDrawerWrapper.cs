using System.Collections.Generic;
using UnityEngine;

namespace FFS.Libraries.StaticEcs.Unity.Editor {
    internal class EventDrawerWrapper : ScriptableObject {
        [SerializeReference] public IEvent value;
#if UNITY_6000_4_OR_NEWER
        private static readonly Dictionary<EntityId, EventDrawerWrapper> _pool = new();
#else
        private static readonly Dictionary<int, EventDrawerWrapper> _pool = new();
#endif
        

        public static EventDrawerWrapper GetFor(
#if UNITY_6000_4_OR_NEWER
            EntityId key
#else
            int key
#endif
            ) 
        {
            if (!_pool.TryGetValue(key, out var w) || w == null) {
                w = CreateInstance<EventDrawerWrapper>();
                w.hideFlags = HideFlags.DontSave;
                _pool[key] = w;
            }
            return w;
        }
    }
}
