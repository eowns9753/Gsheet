using System;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace SheetData.Editor.Generator
{
    public static class TypeFinder
    {
        private static List<Assembly> _assemblies = null;
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
        
        private static void RefreshAssemblies()
        {
            _assemblies = new();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic)
                {
                    bool isTarget = assembly.Location.Contains("Assets") || 
                                    assembly.Location.Contains("Assembly-CSharp") || 
                                    assembly.Location.Contains("Unity.Collections");
                    if (isTarget)
                        _assemblies.Add(assembly);
                }  
            }
        }
        
        public static Type Find(string typeName)
        {
            if(_assemblies == null)
                RefreshAssemblies();
            if(_cachedTypes.TryGetValue(typeName, out Type result))
                return result;
            foreach (var ass in _assemblies)
            {
                var export = ass.ExportedTypes;
                foreach (var type in export)
                {
                    if (type.Name == typeName)
                    {
                        _cachedTypes.Add(typeName, type);
                        return type;
                    }
                }
            }
            return result;
        }
        
    }
}