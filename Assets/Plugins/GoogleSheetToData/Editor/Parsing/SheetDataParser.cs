using System;
using System.Collections.Generic;
using System.Linq;
using SheetData.Editor.Generator;
using SheetData.Scripts.Parsing;
using UnityEngine;

namespace SheetData.Editor.Parsing
{
    public static class SheetDataParser
    {
        private static Dictionary<string, ISheetParserable> _parserMap = null;
        
        public static IParserFormatter GetParser(Type dataType, string typeString)
        {
            if(_parserMap == null || _parserMap.Count == 0)
                RefreshParser();

            var formatter = ParserFormatter.GetFormatter(dataType, typeString);
            return formatter;
        }

        private static void RefreshParser()
        {
            _parserMap = new();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic)
                {
                    string sheet = nameof(SheetData);
                    string sheetEdit = $"{sheet}.Editor";
                    bool isTarget = assembly.Location.Contains("Assembly-CSharp") || 
                                    assembly.Location.Contains(sheetEdit) || 
                                    assembly.Location.Contains(sheet);
                    if (isTarget)
                    {
                        foreach (var type in assembly.GetExportedTypes())
                        {
                            if (typeof(ISheetParserable).IsAssignableFrom(type))
                            {
                                var args = GetGenerics(type);
                                if (args != null && args.Length > 0 && args[0].Name != "T")
                                {
                                    var arg = args[0];
                                    var typeString = GetTypeString(args[0]);
                                    var instance = (ISheetParserable)Activator.CreateInstance(type);
                                    bool success = _parserMap.TryAdd(typeString, instance);
                                    if (!success)
                                        throw new Exception($"{arg.Name} 에 대한 처리가 중복되었습니다");
                                }
                            }
                        }
                    }
                }  
            }
            Debug.Log($"Parseable {_parserMap.Count} Types");
        }

        private static Type[] GetGenerics(Type targetType)
        {
            while (targetType != null && targetType != typeof(object))
            {
                if (targetType.IsGenericType)
                    return targetType.GetGenericArguments();
                targetType = targetType.BaseType;
            }
            return null;
        }

        private static string GetTypeString(Type targetType)
        {
            if (targetType.IsGenericType)
            {
                var genericTypes = targetType.GetGenericArguments();
                var str = targetType.Name.Split('`').First();
                
                if (genericTypes.Length == 1)
                {
                    return $"{str}<{genericTypes[0].Name}>";
                }
                else if (genericTypes.Length == 2)
                {
                    return $"{str}<{genericTypes[0].Name},{genericTypes[1].Name}>";
                }
            }
            else if (targetType.IsArray)
            {
                return $"{targetType.Name}[]";
            }
            else
            {
                return targetType.Name;
            }
            return "";
        }
    }
}