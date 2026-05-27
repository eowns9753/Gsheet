using SheetData;
using SheetData.Localize;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    [SerializeField] private Button _btnEn;
    [SerializeField] private Button _btnJp;
    [SerializeField] private Button _btnKr;
    
    void Start()
    {
        _btnEn.onClick.AddListener(() =>
        {
            LocalizeManager.Instance.SetLanguage(LangCode.EN);
        });
        _btnJp.onClick.AddListener(() =>
        {
            LocalizeManager.Instance.SetLanguage(LangCode.JP);
        });
        _btnKr.onClick.AddListener(() =>
        {
            LocalizeManager.Instance.SetLanguage(LangCode.KR);
        });
    }
}
