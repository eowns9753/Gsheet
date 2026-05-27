using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Localize.Elements;

namespace SheetData.Localize
{
    /// <summary>
    /// 자동으로 생성된 Gsheet의 로컬라이징 데이터를 찾아 로컬라이징 데이터로 활용할수있게 합니다.
    /// </summary>
    public static class LocalizeSheetBinder
    {
        private const string GeneratorAssemblyName = "Assembly-CSharp";
        
        private static Dictionary<LangCode, FieldInfo> _langPropertyAccessor;
        private static object _gsheetInstance;
        private static PropertyInfo _gsheetLocalizeDictionaryField;

        private static bool CheckAndInitialize()
        {
            if (_gsheetInstance == null)
            {
                _langPropertyAccessor = new();
                var gsheetData = SheetDataSettingScriptable.Instance;
                if (string.IsNullOrEmpty(gsheetData.LocalizeSheetName))
                {
                    _gsheetInstance = null;
                }
                else
                {
                    var gsheetType = Type.GetType($"{gsheetData.GeneratorNameSpace}.Gsheet, {GeneratorAssemblyName}");
                    _gsheetInstance = gsheetType.GetProperty("Instance").GetValue(null);
                    _gsheetLocalizeDictionaryField = gsheetType.GetProperty(gsheetData.LocalizeSheetName);
                    var localizeType = Type.GetType($"{gsheetData.GeneratorNameSpace}.{gsheetData.LocalizeSheetName}, {GeneratorAssemblyName}");
                    var allfields = localizeType.GetFields();
                    EnumCache<LangCode> langCodeCache = new EnumCache<LangCode>();
                    foreach (var field in allfields)
                    {
                        if (langCodeCache.HasEnum(field.Name))
                            _langPropertyAccessor.Add(langCodeCache.ToEnum(field.Name), field);
                    }
                }
            }

            return _gsheetInstance != null;
        }
        
        public static string GetLocalizeString(LangCode lang, string key)
        {
            string result = "";
            if (CheckAndInitialize())
            {
                var dic = (Dictionary<string, object>)_gsheetLocalizeDictionaryField.GetValue(_gsheetInstance);
                if(dic.TryGetValue(key, out var value))
                    result = (string)_langPropertyAccessor[lang].GetValue(value);
            }
            return result;
        }
        
        public static string GetLocalizeString(string key)
        {
            if (CheckAndInitialize())
            {
                var lanCode = _langPropertyAccessor.Count > 0 ? _langPropertyAccessor.First().Key : 0;
                return GetLocalizeString(lanCode, key);
            }
            return "";
        }
    }
}