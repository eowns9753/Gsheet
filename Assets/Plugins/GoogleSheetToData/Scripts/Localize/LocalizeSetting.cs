using System;
using UnityEngine;

namespace SheetData.Localize
{
    [Serializable]
    public class LocalizeSetting
    {
        [SerializeField] private string _sheetName = "Localize";
        [SerializeField, HideInInspector] private string[] _languageCodes = new string[0];
        
        public string SheetName => _sheetName;
        public string[] LanguageCodes { get => _languageCodes; set => _languageCodes = value; }
    }
}
