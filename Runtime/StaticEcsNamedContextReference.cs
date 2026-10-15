using UnityEngine;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace FFS.Libraries.StaticEcs.Unity {
    #if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    #endif
    [DefaultExecutionOrder(short.MinValue)]
    public abstract class StaticEcsNamedContextReference : MonoBehaviour {
        [SerializeField]
        private string _key;
        [SerializeField]
        private RegistrationType _registrationType = RegistrationType.OnAwake;

        public string Key() {
            return _key;
        }

        void Awake() {
            if (_registrationType == RegistrationType.OnAwake) {
                World.SetResource(_key, new GameObjectResource(gameObject));
            }
        }

        void OnEnable() {
            if (_registrationType == RegistrationType.OnEnable) {
                World.SetResource(_key, new GameObjectResource(gameObject));
            }
        }

        void OnDisable() {
            if (_registrationType == RegistrationType.OnEnable) {
                World.RemoveResource(_key);
            }
        }

        void OnDestroy() {
            if (_registrationType == RegistrationType.OnAwake) {
                World.RemoveResource(_key);
            }
        }

        enum RegistrationType {
            OnAwake, OnEnable
        }
    }
}
