using System.Data;
using System.Threading.Tasks;
using SheetData.Editor.DownLoader;
using SheetData.Editor.Generator;
using SheetData.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace SheetData.Editor
{
    [CustomEditor(typeof(SheetDataSettingScriptable))]
    [CanEditMultipleObjects]
    public class SheetDataSettingScriptableEditor : UnityEditor.Editor
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (SheetDataSettingScriptable.Instance == null)
            {
                string assetPath = AssetDatabase.GetAssetPath(
                    MonoScript.FromScriptableObject(CreateInstance(typeof(SheetDataSettingScriptable))));
                var finded = IOUtils.GetAssetsForFolder<SheetDataSettingScriptable>(IOUtils.GetDirectory(assetPath));
                if (finded.Count == 0)
                {
                    var scriptablePath = IOUtils.GetDirectory(assetPath) + SheetDataSettingScriptable.FileName;
                    if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(scriptablePath) == null)
                        ScriptableCreator.Create<SheetDataSettingScriptable>(scriptablePath);
                }
            }
        }

        async void GenerateData(SheetDataSettingScriptable target)
        {
            await RefreshSheetNames(target);
            for (int i = 0; i < target.SheetInfos.Count; i++)
            {
                var sheetData = await SheetLoader.Load(target.SheetID, target.SheetInfos[i]);
                TypeGenerator.Generator(sheetData, target.CodeGeneratorPos, target.GeneratorNameSpace);
                AssetDatabase.Refresh();
                
            }
        }
        
        async Task RefreshSheetNames(SheetDataSettingScriptable target)
        {
            var names = await SheetLoader.GetSheetNames(target.SheetID);
            if (names.Count > 0)
            {
                target.SheetInfos.Clear();
                target.SheetInfos.AddRange(names);
                EditorUtility.SetDirty(target);
            }
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SheetDataSettingScriptable scriptable = (SheetDataSettingScriptable)target;
            if (scriptable == null)
                return;
            
            GUILayout.Label("asdasdsada");

            if (GUILayout.Button("OpenSheet"))
            {
                Application.OpenURL($"https://docs.google.com/spreadsheets/d/{scriptable.SheetID}/edit");
            }
            if (GUILayout.Button("GenerateData"))
            {
                GenerateData(scriptable);
            }
            
            if (GUILayout.Button("StartDebug"))
            {
                RefreshSheetNames(scriptable);
                scriptable.StartDebug();
            }
        }
    }
}