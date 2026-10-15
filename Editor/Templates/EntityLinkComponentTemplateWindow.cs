using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FFS.Libraries.StaticEcs.Unity.Editor {
    public class EntityLinkComponentTemplateWindow : EditorWindow {
        string[] names = {"Link"};
        string path;
        Vector2 scroll;

        string nameSpace;
        string worldName = "World";
        string worldTypeName = "World";
        Type worldType = typeof(World);

        bool withHooks = false;

        bool withExtensions = true;
        bool refMethod = true;
        bool addMethod = true;
        bool setMethod = true;
        bool hasMethod = true;
        bool deleteMethod = true;
        bool copyMethod = true;
        bool moveMethod = true;
        bool setLinkMethod = true;


        bool withColor = true;
        Color color = Color.white;

        bool withGroup = false;
        string groupName = "";

        [MenuItem("Assets/Create/Static ECS/Entity Link-Components", false, -226)]
        static void ShowWindow() {
            var window = GetWindow<EntityLinkComponentTemplateWindow>(true, "Entity Link-Component template");
            window.minSize = new Vector2(300, 200);
            window.path = AssetPath();
            window.nameSpace = EditorSettings.projectGenerationRootNamespace.Trim();
            Drawer.openHideFlags.Add((typeof(string[]).FullName + "Links" + 10).GetHashCode());
        }

        void OnGUI() {
            scroll = EditorGUILayout.BeginScrollView(scroll);


            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("World:", Ui.WidthLine(60));
                EditorGUILayout.LabelField(worldName, Ui.LabelStyleThemeBold);
                if (worldType != null && names.Length > 0 && GUILayout.Button("Create Link-Components", Ui.ButtonStyleYellow)) {
                    CreateFiles();
                    Close();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            nameSpace = EditorGUILayout.TextField("Namespace", nameSpace);

            EditorGUILayout.Space(10);
            Drawer.DrawStringArray("Links", ref names);
            for (var i = 0; i < names.Length; i++) {
                ref var val = ref names[i];
                if (string.IsNullOrEmpty(val)) {
                    val = "Link";
                }
            }

            EditorGUILayout.Space(10);
            withHooks = EditorGUILayout.Toggle("Hooks", withHooks);

            EditorGUILayout.Space(10);
            withColor = EditorGUILayout.Toggle("Editor color", withColor);
            if (withColor) {
                EditorGUI.indentLevel++;
                color = EditorGUILayout.ColorField("Color", color);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);
            withGroup = EditorGUILayout.Toggle("Editor group", withGroup);
            if (withGroup) {
                EditorGUI.indentLevel++;
                groupName = EditorGUILayout.TextField("Group name", groupName);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);
            withExtensions = EditorGUILayout.Toggle("Extensions", withExtensions);
            if (withExtensions) {
                EditorGUI.indentLevel++;
                refMethod = EditorGUILayout.Toggle("Ref", refMethod);
                addMethod = EditorGUILayout.Toggle("Add", addMethod);
                setMethod = EditorGUILayout.Toggle("Set", setMethod);
                hasMethod = EditorGUILayout.Toggle("Has", hasMethod);
                deleteMethod = EditorGUILayout.Toggle("Delete", deleteMethod);
                copyMethod = EditorGUILayout.Toggle("Copy", copyMethod);
                moveMethod = EditorGUILayout.Toggle("Move", moveMethod);
                setLinkMethod = EditorGUILayout.Toggle("Set link", setLinkMethod);
                EditorGUI.indentLevel--;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
        }

        public void CreateFiles() {
            foreach (var componentName in names) {
                var fileName = $"{path}/{componentName}.cs";
                var text = CreateTemplate(componentName);
                try {
                    File.WriteAllText(AssetDatabase.GenerateUniqueAssetPath(fileName), text);
                }
                catch (Exception ex) {
                    Debug.LogError(ex.Message);
                }
            }

            AssetDatabase.Refresh();
        }

        static string AssetPath() {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.Contains(Selection.activeObject)) {
                if (!AssetDatabase.IsValidFolder(path)) {
                    path = Path.GetDirectoryName(path);
                }
            } else {
                path = "Assets";
            }

            return path;
        }

        public string CreateTemplate(string linkName) {
            var sb = new StringBuilder();
            var pad = string.IsNullOrEmpty(nameSpace) ? "" : "    ";
            sb.AppendLine("using System;");
            sb.AppendLine("using FFS.Libraries.StaticEcs;");
            sb.AppendLine("using FFS.Libraries.StaticEcs.Unity;", withColor);
            sb.AppendLine($"#if ENABLE_IL2CPP");
            sb.AppendLine($"using Unity.IL2CPP.CompilerServices;");
            sb.AppendLine($"#endif");
            sb.AppendLine("using System.Runtime.CompilerServices;", withExtensions);
            sb.AppendLine("using static System.Runtime.CompilerServices.MethodImplOptions;", withExtensions);
            sb.AppendLine($"using static FFS.Libraries.StaticEcs.World;", withExtensions);
            sb.AppendLine();
            sb.AppendLine($"namespace {nameSpace} {{", !string.IsNullOrEmpty(nameSpace));

            sb.AppendLine($"{pad}#if ENABLE_IL2CPP");
            sb.AppendLine($"{pad}[Il2CppSetOption(Option.NullChecks, false)]");
            sb.AppendLine($"{pad}[Il2CppSetOption(Option.ArrayBoundsChecks, false)]");
            sb.AppendLine($"{pad}#endif");
            sb.AppendLine($"{pad}[Serializable]");
            sb.AppendLine($"{pad}[StaticEcsEditorColor(" +
                $"{color.r.ToString("0.###", CultureInfo.InvariantCulture)}f, " +
                $"{color.g.ToString("0.###", CultureInfo.InvariantCulture)}f, " +
                $"{color.b.ToString("0.###", CultureInfo.InvariantCulture)}f)]", withColor);
            if (withGroup) {
                if (withColor) {
                    sb.AppendLine($"{pad}[StaticEcsEditorGroup(\"{groupName}\", " +
                        $"{color.r.ToString("0.###", CultureInfo.InvariantCulture)}f, " +
                        $"{color.g.ToString("0.###", CultureInfo.InvariantCulture)}f, " +
                        $"{color.b.ToString("0.###", CultureInfo.InvariantCulture)}f)]");
                } else {
                    sb.AppendLine($"{pad}[StaticEcsEditorGroup(\"{groupName}\")]");
                }
            }
            sb.AppendLine($"{pad}public struct {linkName} : ILinkType, ILinkConfig<{linkName}> {{");
            sb.AppendLine($"{pad}    public ComponentTypeConfig<World.Link<{linkName}>> Config()");
            sb.AppendLine($"{pad}        => new(guid: new(\"{GUID.Generate().ToString()}\"));");
            sb.AppendLine();
            if (withHooks) {
                sb.AppendLine($"{pad}    public void OnAdd(World.Entity self, EntityGID link) {{");
                sb.AppendLine($"{pad}        // TODO implement this");
                sb.AppendLine($"{pad}    }}");
                sb.AppendLine();
                sb.AppendLine($"{pad}    public void OnDelete(World.Entity self, EntityGID link, HookReason reason) {{");
                sb.AppendLine($"{pad}        // TODO implement this");
                sb.AppendLine($"{pad}    }}");
                sb.AppendLine();
                sb.AppendLine($"{pad}    public void CopyTo(World.Entity self, World.Entity other, EntityGID link) {{");
                sb.AppendLine($"{pad}        // TODO implement this");
                sb.AppendLine($"{pad}    }}");
            }
            sb.AppendLine($"{pad}}}");
            if (withExtensions) {
                sb.AppendLine();
                sb.AppendLine($"{pad}#if ENABLE_IL2CPP");
                sb.AppendLine($"{pad}[Il2CppSetOption(Option.NullChecks, false)]");
                sb.AppendLine($"{pad}[Il2CppSetOption(Option.ArrayBoundsChecks, false)]");
                sb.AppendLine($"{pad}#endif");
                sb.AppendLine($"{pad}public static class {linkName}ExtensionsFor{worldTypeName} {{");
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static ref Link<{linkName}> {linkName}Link(this Entity entity) => ref Components<Link<{linkName}>>.Instance.Ref(entity);\n", refMethod);
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static ref Link<{linkName}> Add{linkName}Link(this Entity entity) => ref Components<Link<{linkName}>>.Instance.Add(entity);\n", addMethod);
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static void Set{linkName}Link(this Entity entity, Link<{linkName}> value) => Components<Link<{linkName}>>.Instance.Set(entity, value);\n", setMethod);
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static void Set{linkName}Link(this Entity entity, EntityGID link) => Components<Link<{linkName}>>.Instance.Set(entity, link);\n", setLinkMethod);
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static bool Has{linkName}Link(this Entity entity) => Components<Link<{linkName}>>.Instance.Has(entity);\n", hasMethod);
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static bool Delete{linkName}Link(this Entity entity) => Components<Link<{linkName}>>.Instance.Delete(entity);\n", deleteMethod);
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static void Copy{linkName}LinkTo(this Entity entity, Entity dst) => Components<Link<{linkName}>>.Instance.Copy(entity, dst);\n", copyMethod);
                sb.AppendLine($"{pad}    [MethodImpl(AggressiveInlining)]\n{pad}    public static void Move{linkName}LinkTo(this Entity entity, Entity dst) => Components<Link<{linkName}>>.Instance.Move(entity, dst);\n", moveMethod);
                sb.AppendLine($"{pad}}}");
            }
            sb.AppendLine("}", !string.IsNullOrEmpty(nameSpace));

            return sb.ToString();
        }
    }
}
