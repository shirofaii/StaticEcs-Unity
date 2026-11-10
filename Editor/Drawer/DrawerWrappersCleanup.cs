using UnityEditor;
using UnityEngine;

namespace FFS.Libraries.StaticEcs.Unity.Editor {
    /// Drawer wrappers are DontSave ScriptableObjects, so Unity backs them up and restores them across domain reloads.
    /// Their pools are static and start empty after a reload, so restored wrappers are leaked, and restoring their
    /// [SerializeReference] value logs "missing entry for Refid -2". Destroy them all before each reload instead.
    [InitializeOnLoad]
    internal static class DrawerWrappersCleanup {
        static DrawerWrappersCleanup() {
            AssemblyReloadEvents.beforeAssemblyReload += DestroyAll;
        }

        private static void DestroyAll() {
            foreach (var so in Resources.FindObjectsOfTypeAll<ScriptableObject>()) {
                if (so is ComponentDrawerWrapper or BoxedStructDrawerWrapper or ContextDrawerWrapper
#if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
                    or SystemDrawerWrapper
#endif
                   ) {
                    Object.DestroyImmediate(so);
                }
            }
        }
    }
}
