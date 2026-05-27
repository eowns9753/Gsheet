namespace SheetData.Localize.Editor
{
    public class LocalizeStringEditor
    {
        
    }
}


/*


        #region EditorCode
#if UNITY_EDITOR
        private static  GUILayoutOption _width = null;
        [OnInspectorGUI]
        public void DrawGUI()
        {
            _width ??= GUILayout.Width(270);
            
            var originalC = GUI.color;
            GUI.color = Color.cyan;
            if(_localizeKey == "")
            {
                GUILayout.Label("- 번역을 사용하지않습니다.",_width);
            }
            else
            {
                GUILayout.Label("- "+GetString(),_width);
            }
            GUI.color = originalC;
        }

        
        public void DrawSearchKeyGUI(string label)
        {
#if UNITY_EDITOR
            _localizeKey = SirenixEditorFields.TextField(label, _localizeKey);
#endif
        }
#endif
        #endregion
*/