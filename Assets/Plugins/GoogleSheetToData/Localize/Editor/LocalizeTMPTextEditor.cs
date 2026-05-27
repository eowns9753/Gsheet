using Localize;
using Localize.Elements;
using SheetData.SheetData;
using UnityEditor;
using UnityEngine;
using TMPro.EditorUtilities;

[CustomEditor(typeof(TextMeshProLocalizeUGUI), true), CanEditMultipleObjects]
public class LocalizeTMPTextEditor : TMP_EditorPanelUI
{
    static bool LocalizeFoldout = true;
    SerializedProperty localizeKeyProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        // 실제 TextMeshProLocalizeUGUI 클래스에 정의된 필드 이름과 일치해야 합니다.
        // 만약 private 필드에 [SerializeField]를 사용했다면 "m_LocalizeKey" 형태일 수 있습니다.
        localizeKeyProp = serializedObject.FindProperty("LocalizeKey");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Rect rect = EditorGUILayout.GetControlRect(false, 24);
        if (GUI.Button(rect, new GUIContent("<b>Localize(Tofu)</b>"), TMP_UIStyleManager.sectionHeader))
            LocalizeFoldout = !LocalizeFoldout;

        var tmpt = target as TextMeshProLocalizeUGUI;

        // null 체크: SerializedProperty를 사용하면 Unity가 [Serializable] 객체를 자동 생성하지만,
        // 인스턴스가 런타임에 직접 할당되어야 하는 구조라면 아래 코드를 유지합니다.
        if (tmpt.LocalizeKey == null)
            tmpt.LocalizeKey = new LocalizeString();

        if (LocalizeFoldout)
        {
            if (localizeKeyProp != null)
            {
                // true 파라미터를 넘겨 클래스 내부의 하위 프로퍼티를 트리 형태로 모두 그려줍니다.
                EditorGUILayout.PropertyField(localizeKeyProp, new GUIContent("Localize Key"), true);
            }
            else
            {
                EditorGUILayout.HelpBox("LocalizeKey 프로퍼티를 찾을 수 없습니다. 필드 이름을 확인하세요.", MessageType.Warning);
            }
        }

        // GUI 변경 사항을 SerializedObject에 반영 (Undo / Prefab Override 정상 작동 보장)
        bool isModified = serializedObject.ApplyModifiedProperties();

        // 매 프레임 text를 할당하는 대신, 프로퍼티가 변경되었을 때만 텍스트를 갱신하고 Dirty 마킹
        if (isModified)
        {
            tmpt.text = tmpt.LocalizeKey.GetString();
            EditorUtility.SetDirty(tmpt);
        }

        base.OnInspectorGUI();
    }
}