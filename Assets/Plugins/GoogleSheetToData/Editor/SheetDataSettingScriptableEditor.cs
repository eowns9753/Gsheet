using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SheetData.Editor.DownLoader;
using SheetData.Editor.Generator;
using SheetData.Editor.Utils;
using SheetData.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace SheetData.Editor
{
    [CustomEditor(typeof(SheetDataSettingScriptable))]
    [CanEditMultipleObjects]
    public class SheetDataSettingScriptableEditor : UnityEditor.Editor
    {
        private const string LOG_KEY = "GSHEETLOGKEY";
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (SheetDataSettingScriptable.Instance == null)
            {
                ScriptableCreator.Create<SheetDataSettingScriptable>($"Assets/Resources/{SheetDataSettingScriptable.FileName}.asset");
            }
        }

        /// <summary> 데이터를 생성하고 Gsheet 클래스를 생성합니다 </summary>
        async Task GenerateData(SheetDataSettingScriptable target)
        {
            target.OnBeginGenerator();
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
            SheetBinaryWriter writer = SheetBinaryWriter.Create($"Resources/{SheetDataSettingScriptable.BinaryFileName}.bytes");
            writer.Write(sheetDatas.Count);
            foreach (var sheetData in sheetDatas)
            {
                modelMap.Add(sheetData.SheetName, sheetData.ClassGenerator(target.GeneratorNameSpace));
                sheetData.WriteDirect(writer, modelMap[sheetData.SheetName]);
                if (sheetData.SheetName == target.LocalizeSetting.SheetName)
                {
                    target.LocalizeSetting.LanguageCodes = sheetData.Headers.Skip(1).Select(o => o.memberName).ToArray();
                    CreateLocalizeEnums(target);
                }
            }

            int writerSize = writer.Length;
            Debug.Log($"size {writerSize}");
            writer.Save();
            writer.Dispose();
            
            foreach (var sheetData in sheetDatas)
            {
                var generatorCode = modelMap[sheetData.SheetName].Generator();
                if (generatorCode != "")
                {
                    string path = IOUtils.GetSystemPath($"{target.CodeGenerationPath}/{sheetData.SheetName}.cs");
                    IOUtils.SaveFile(path, Encoding.UTF8.GetBytes(generatorCode));
                }
            }
            GSheetModel model = new GSheetModel(sheetDatas.ToArray(), target.GeneratorNameSpace);
            IOUtils.SaveFile(IOUtils.GetSystemPath($"{target.CodeGenerationPath}/{GSheetModel.NAME}.cs"), 
                Encoding.UTF8.GetBytes(model.Generator()));
            EditorPrefs.SetString(LOG_KEY, $"BinarySize - {writerSize:N0} bytes, Updated - {DateTime.Now.ToString()}");
            AssetDatabase.Refresh();
            target.OnEndGenerator();
        }
        
        /// <summary> 시트의 이름들을 갱신하고 Scriptable에 메타데이터로 등록합니다 </summary>
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

        /// <summary> 지정된 Localize Sheet를 참조해 번역대상 언어코드를 생성합니다. </summary>
        private void CreateLocalizeEnums(SheetDataSettingScriptable target)
        {
            if(string.IsNullOrEmpty(target.LocalizeSetting.SheetName))
                return;
            var langs = Enum.GetNames(typeof(LangCode)).ToHashSet();
            foreach (var code in target.LocalizeSetting.LanguageCodes)
                if (langs.Contains(code))
                    langs.Remove(code);
                else 
                    langs.Add(code);
            if (langs.Count > 0)
            {
                string targetDirectory = "Assets/Plugins/GoogleSheetToData/";
                string[] guids = AssetDatabase.FindAssets("LangCode t:script");

                if (guids.Length > 0)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    targetDirectory = Path.GetDirectoryName(assetPath).Replace("\\", "/") + "/";
                }
                else
                {
                    Debug.LogWarning("LangCode.cs 파일을 찾을 수 없어 기본 경로에 생성합니다.");
                }
                EnumCreator creator = new EnumCreator("LangCode", targetDirectory, "SheetData");
                foreach (var code in target.LocalizeSetting.LanguageCodes)
                    creator.AddEnum(code);
                creator.Generator();
            }
        }
        
        public override void OnInspectorGUI()
        {
            GUILayout.Label("GSheet Setting", EditorStyles.largeLabel);
            base.OnInspectorGUI();
            SheetDataSettingScriptable scriptable = (SheetDataSettingScriptable)target;
            if (scriptable == null)
                return;
            GUILayout.Space(20);
            GUILayout.Label("- LastUpdateInfo \n" + EditorPrefs.GetString(LOG_KEY, "Empty Log"));
            GUILayout.Space(20);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OpenSheet"))
            {
                Application.OpenURL($"https://docs.google.com/spreadsheets/d/{scriptable.SheetID}/edit");
            }
            if (GUILayout.Button("Generator"))
            {
                _ = GenerateData(scriptable);
            }
            GUILayout.EndHorizontal();
        }

     
        
        [MenuItem("Tools/Gsheet Info")]
        public static void ShowSetting()
        {
            EditorWindow.CreateWindow<GsheetSettingWindow>().Show();
        }

        public static SheetDataSettingScriptable GetScriptable()
        {
            string[] guids = AssetDatabase.FindAssets("t:SheetDataSettingScriptable");
            var asset = AssetDatabase.LoadAssetAtPath<SheetDataSettingScriptable>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return asset;
        }
    }
    
    public class GsheetSettingWindow : EditorWindow
    {
        public void CreateGUI()
        {
            var asset = SheetDataSettingScriptableEditor.GetScriptable();
            titleContent = new GUIContent(nameof(GsheetSettingWindow));
            // 래퍼 요소 생성: 이 한 줄로 인스펙터가 통째로 들어옵니다.
            InspectorElement inspector = new InspectorElement(asset);
            rootVisualElement.Add(inspector);
        }
    }
}