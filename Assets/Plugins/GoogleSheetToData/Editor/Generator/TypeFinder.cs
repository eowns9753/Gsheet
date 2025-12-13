using System;
using System.Collections.Generic;
using UnityEngine;

namespace SheetData.Editor.Generator
{
    public static class TypeFinder
    {
        private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>()
        {
            {"string", typeof(string)},
            {"int", typeof(int)},
            {"float", typeof(float)},
            {"Vector2", typeof(Vector2)},
            {"decimal", typeof(decimal)},
            {"char", typeof(char)},
            {"bool", typeof(bool)},
            //{"", typeof(FixedString)}
        };
        public static Type Find(string typeName)
        {
            if(_cachedTypes.TryGetValue(typeName, out Type result))
                return result;
            
            if (result == null)
            {//Deep find
                
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (!assembly.IsDynamic && assembly.Location.Contains("Assets"))
                    {
                        var  types = assembly.ExportedTypes;
                        foreach (var type in types)
                        {
                            if (type.Name == typeName)
                                return type;
                        }
                    }
                    
                }
            }

            return result;
        }
    }
}