using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Localize.Elements;
using Unity.VisualScripting.FullSerializer.Internal;

namespace SheetData.Localize
{
    /// <summary>
    /// 자동으로 생성된 Gsheet의 로컬라이징 데이터를 찾아 로컬라이징 데이터로 활용할수있게 합니다.
    /// </summary>
    public static class LocalizeSheetBinder
    {
        private const string GeneratorAssemblyName = "Assembly-CSharp";
        
        private static Dictionary<LangCode, PropertyInfo> _langPropertyAccessor;
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
                    var allProperties = localizeType.GetDeclaredProperties();
                    EnumCache<LangCode> langCodeCache = new EnumCache<LangCode>();
                    foreach (var property in allProperties)
                    {
                        if (langCodeCache.HasEnum(property.Name))
                            _langPropertyAccessor.Add(langCodeCache.ToEnum(property.Name), property);
                    }
                }
            }
            return _gsheetInstance != null;
        }
        
        public static string GetLocalizeString(LangCode lang, string key)
        {
            string result = key;
            if (CheckAndInitialize())
            {
                var dic = (System.Collections.IDictionary)_gsheetLocalizeDictionaryField.GetValue(_gsheetInstance);
                if (dic.Contains(key))
                    result = (string)_langPropertyAccessor[lang].GetValue(dic[key]);
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
            return key;
        }
    }
}