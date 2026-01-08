using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LWSerializer;
using SheetData.Editor.DownLoader;
using SheetData.Editor.Generator;
using SheetData.Editor.Utils;
using SheetData.IO;
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
            
            List<SheetRawData> sheetDatas = new();
            for (int i = 0; i < target.SheetInfos.Count; i++)
            {
                var raw = await SheetLoader.Load(target.SheetID, target.SheetInfos[i]);
                sheetDatas.Add(raw);
                foreach (var header in raw.Headers)
                {
                    if(header.IsMissingType)
                        throw new Exception($"Header {header.originalText} is missing type");
                }        
            }

            Dictionary<string, TypeModel> modelMap = new Dictionary<string, TypeModel>();
            SheetBinaryWriter writer = SheetBinaryWriter.Create(SheetDataSettingScriptable.BinaryFileName);
            writer.Write(sheetDatas.Count);
            foreach (var sheetData in sheetDatas)
            {
                modelMap.Add(sheetData.SheetName, sheetData.ClassGenerator(target.GeneratorNameSpace));
                sheetData.WriteDirect(writer, modelMap[sheetData.SheetName]);
            }
            Debug.Log($"size {writer.Length}");
            writer.Save();
            writer.Dispose();
            
            foreach (var sheetData in sheetDatas)
            {
                var generatorCode = modelMap[sheetData.SheetName].Generator();
                if (generatorCode != "")
                {
                    string path = IOUtils.GetSystemPath($"{target.CodeGeneratorPos}/{sheetData.SheetName}.cs");
                    IOUtils.SaveFile(path, Encoding.UTF8.GetBytes(generatorCode));
                }
            }
            GSheetModel model = new GSheetModel(sheetDatas.ToArray(), target.GeneratorNameSpace);
            IOUtils.SaveFile(IOUtils.GetSystemPath($"{target.CodeGeneratorPos}/{GSheetModel.ClassName}.cs"), 
                Encoding.UTF8.GetBytes(model.Generator()));
            AssetDatabase.Refresh();
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

        void LoadTest(SheetDataSettingScriptable target)
        {
            SheetBinaryReader reader = SheetBinaryReader.Create(SheetDataSettingScriptable.BinaryFileName);
            reader.Read(out int sheetCount);
            for (int i = 0; i < sheetCount; i++)
            {
                var data = SheetDataHelper.ReadSheet(reader);
            }
            
            reader.Dispose();
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
                LoadTest(scriptable);
            }
        }
    }
}