using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace UniGears.AsmdefKit.Editor
{
    internal static class AsmdefEditorUtils
    {
        private const string SORT_ALL_ASMDEF_REFERENCES_IN_ASSETS = "UniGears/AsmdefKit/Sort All Asmdef References In Assets";
        private const string SORT_ALL_ASMDEF_REFERENCES_RECURSIVELY = "Assets/UniGears/AsmdefKit/Sort All Asmdef References Recursively";
        private const string SORT_ASMDEF_REFERENCES = "Assets/UniGears/AsmdefKit/Sort Asmdef References";
        private const string SORT_COMPLETED = "Completed";

        [MenuItem(SORT_ALL_ASMDEF_REFERENCES_IN_ASSETS, false, 0)]
        private static void SortAllAsmdefReferencesInAssets()
        {
            var allAssemblyDefinitionAssetPath = AssetDatabase
                    .GetAllAssetPaths()
                    .Where(x => x.EndsWith(".asmdef") && x.StartsWith("Assets/"))
                    .Distinct()
                    .ToArray();

            SortGroup(allAssemblyDefinitionAssetPath);
        }

        [MenuItem(SORT_ALL_ASMDEF_REFERENCES_RECURSIVELY, true)]
        private static bool ValidateSortAllAsmdefReferencesRecursively()
        {
            return Selection.objects.OfType<DefaultAsset>().Any(x => AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(x)));
        }

        [MenuItem(SORT_ALL_ASMDEF_REFERENCES_RECURSIVELY, false, 1)]
        private static void SortAllAsmdefReferencesRecursively()
        {
            var selectedPaths = Selection.assetGUIDs
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(AssetDatabase.IsValidFolder)
                    .ToArray();

            var assemblyDefinitionAssetArray = selectedPaths
                    .SelectMany(x => AssetDatabase.FindAssets("t:AssemblyDefinitionAsset", new[] { x }))
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(x => x.EndsWith(".asmdef") && x.StartsWith("Assets/"))
                    .Distinct()
                    .ToArray();

            SortGroup(assemblyDefinitionAssetArray);
        }

        [MenuItem(SORT_ASMDEF_REFERENCES, true)]
        private static bool ValidateSortAsmdefReferences()
        {
            return Selection.objects.OfType<AssemblyDefinitionAsset>().Any();
        }

        [MenuItem(SORT_ASMDEF_REFERENCES, false, 2)]
        private static void SortAsmdefReferences()
        {
            var assemblyDefinitionAssetArray = Selection.objects
                    .OfType<AssemblyDefinitionAsset>()
                    .Select(x => AssetDatabase.GetAssetPath(x))
                    .Where(x => x.EndsWith(".asmdef") && x.StartsWith("Assets/"))
                    .Distinct()
                    .ToArray();

            SortGroup(assemblyDefinitionAssetArray);
        }

        private static void SortGroup(string[] assetPaths)
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                var length = assetPaths.Length;

                for (var i = 0; i < length; i++)
                {
                    var number = i + 1;
                    var assetPath = assetPaths[i];

                    EditorUtility.DisplayProgressBar
                    (
                        title: "Sort Assembly Definition References",
                        info: $"{number} / {length}    {assetPath}",
                        progress: (float)number / length
                    );

                    AsmdefReferencesSorter.Sort(assetPath);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("", $"{SORT_COMPLETED}", "OK");
        }
    }
}
