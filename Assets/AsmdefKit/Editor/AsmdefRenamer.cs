using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniGears.AsmdefKit.Editor
{
    internal class AsmdefRenamer : EditorWindow
    {
        private string stringToReplace = "";
        private string replacementString = "";

        [MenuItem("UniGears/AsmdefKit/Renamer")]
        public static void ShowWindow()
        {
            GetWindow<AsmdefRenamer>("Asmdef Renamer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Asmdef File Renamer Tool", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // Function description
            EditorGUILayout.HelpBox("This tool allows you to rename Assembly Definition (.asmdef) files in your project. " +
                                   "It will search for all .asmdef files under the Assets folder and replace specified strings " +
                                   "in both file contents and filenames.", MessageType.Info);

            GUILayout.Space(10);

            stringToReplace = EditorGUILayout.TextField("String to Replace:", stringToReplace);
            replacementString = EditorGUILayout.TextField("Replacement String:", replacementString);

            GUILayout.Space(10);

            GUI.enabled = !string.IsNullOrEmpty(stringToReplace) && !string.IsNullOrEmpty(replacementString);

            if (GUILayout.Button("Execute Rename"))
            {
                RenameAsmdefFiles();
            }

            GUI.enabled = true;

            GUILayout.Space(10);
            GUILayout.Label("Note: This operation will modify all .asmdef files containing the specified string", EditorStyles.helpBox);
        }

        private void RenameAsmdefFiles()
        {
            // Find all .asmdef files
            string[] asmdefGuids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");
            int processedFiles = 0;

            foreach (string guid in asmdefGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Ensure file is under Assets directory
                if (!assetPath.StartsWith("Assets/"))
                    continue;

                string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));

                if (File.Exists(fullPath))
                {
                    string content = File.ReadAllText(fullPath);

                    // Check if contains string to replace
                    if (content.Contains(stringToReplace))
                    {
                        string newContent = content.Replace(stringToReplace, replacementString);
                        File.WriteAllText(fullPath, newContent);
                        processedFiles++;
                        Debug.Log($"Updated file content: {assetPath}");

                        // Check if filename also needs to be renamed
                        string fileName = Path.GetFileNameWithoutExtension(assetPath);
                        if (fileName.Contains(stringToReplace))
                        {
                            string newFileName = fileName.Replace(stringToReplace, replacementString);
                            string newAssetPath = Path.Combine(Path.GetDirectoryName(assetPath), newFileName + ".asmdef");

                            string error = AssetDatabase.MoveAsset(assetPath, newAssetPath);
                            if (string.IsNullOrEmpty(error))
                            {
                                Debug.Log($"Renamed file: {assetPath} -> {newAssetPath}");
                            }
                            else
                            {
                                Debug.LogError($"Failed to rename file: {error}");
                            }
                        }
                    }
                }
            }

            if (processedFiles > 0)
            {
                AssetDatabase.Refresh();
                Debug.Log($"Rename completed! Processed {processedFiles} files.");
                EditorUtility.DisplayDialog("Complete", $"Rename completed! Processed {processedFiles} files.", "OK");
            }
            else
            {
                Debug.Log("No .asmdef files found containing the specified string.");
                EditorUtility.DisplayDialog("Complete", "No .asmdef files found containing the specified string.", "OK");
            }
        }
    }
}