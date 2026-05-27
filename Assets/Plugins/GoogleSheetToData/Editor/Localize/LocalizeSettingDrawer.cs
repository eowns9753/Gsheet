using SheetData.Localize;
using UnityEditor;
using UnityEngine;

namespace SheetData.Editor.Localize
{
    [CustomPropertyDrawer(typeof(LocalizeSetting))]
    public class LocalizeSettingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 텍스트 필드(1줄) + 프리뷰 라벨(1줄) + 줄 사이 여백(2px)
            return EditorGUIUtility.singleLineHeight * 2 + 2;
        }
    }
}

/*

   private void DrawLocalizeInspector(SheetDataSettingScriptable scriptable = null)
        {
            GUILayout.Label($"{scriptable.LocalizeSheetName} Sheet {scriptable.LocalizeLanguageCodes.Length} translations in use");
        }
        //Set Font Set
*/