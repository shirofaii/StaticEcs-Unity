using UnityEditor;
using UnityEngine;

namespace FFS.Libraries.StaticEcs.Unity.Editor {
    [CustomEditor(typeof(StaticEcsEntityProvider), true), CanEditMultipleObjects]
    public class StaticEcsEntityProviderEditor : UnityEditor.Editor {

        private const string FoldoutKeyPrefix = "StaticEcsEntityProviderFoldout_";

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI() {
            if (targets.Length <= 1) {
                DrawProvider((StaticEcsEntityProvider) target);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Selected {targets.Length} entity providers", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            for (var i = 0; i < targets.Length; i++) {
                var provider = (StaticEcsEntityProvider) targets[i];
#if UNITY_6000_4_OR_NEWER
                var key = FoldoutKeyPrefix + provider.GetEntityId();
#else
                var key = FoldoutKeyPrefix + provider.GetInstanceID();
#endif
                var expanded = SessionState.GetBool(key, false);
                var newExpanded = EditorGUILayout.Foldout(expanded, provider.name, true);
                if (newExpanded != expanded) {
                    SessionState.SetBool(key, newExpanded);
                    expanded = newExpanded;
                }

                if (expanded) {
                    DrawProvider(provider);
                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawProvider(StaticEcsEntityProvider provider) {
            if (!provider.EntityIsActual()) {
                DrawDefaultInspector();
            }

            Drawer.DrawEntity(provider, DrawMode.Inspector);
        }
    }
}
