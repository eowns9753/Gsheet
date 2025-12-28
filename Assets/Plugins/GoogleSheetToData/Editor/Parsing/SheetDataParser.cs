using System;
using System.Collections.Generic;
using SheetData.Scripts.Parsing;

namespace SheetData.Editor.Parsing
{
    public static class SheetDataParser
    {
        private static Dictionary<Type, ISheetParserable> _parserMap = new Dictionary<Type, ISheetParserable>();
        
        public static ISheetParserable GetParser(Type type)
        {
            if (!_parserMap.TryGetValue(type, out ISheetParserable parser))
                _parserMap.Add(type, parser = CreateParser(type));
            return parser;
        }

        private static ISheetParserable CreateParser(Type parsingResultType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic)
                {
                    bool isTarget = assembly.Location.Contains("Assembly-CSharp") || 
                                    assembly.Location.Contains("SheetData.Editor");
                    if (isTarget)
                    {
                        foreach (var type in assembly.GetExportedTypes())
                        {
                            if (typeof(ISheetParserable).IsAssignableFrom(type) && type.GenericTypeArguments.Length >0)
                            {
                                if (type.GenericTypeArguments[0] == parsingResultType)
                                    return (ISheetParserable)Activator.CreateInstance(type);
                            }
                        }
                    }
                }  
            }
            throw new Exception("No parser found for type: " + parsingResultType.Name + " need ISheetParserable<T> interface");
        }
    }
}