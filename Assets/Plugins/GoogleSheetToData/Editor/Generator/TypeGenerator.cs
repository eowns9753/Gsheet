using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rui.IO.Serialization;
using Scriban;
using SheetData.Editor.DownLoader;
using SheetData.Editor.Utils;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace SheetData.Editor.Generator
{
    internal class TypeGenerator
    {
        private static readonly Template TEMPLATE = Template.Parse(Model.Template_Class);
        private HashSet<string> _missingTypes = new ();
        private HashSet<string> _namespaceChain = new();
        
        public string Generator(SheetRawData sheetData, string nameSpace)
        {
            if (!sheetData.IsValidation())
                return "";
            var model = CreateModel(sheetData, nameSpace);
            if (_missingTypes.Count > 0)
            {
                DebugLog_MissionTypes(sheetData);
                return "";
            }
            else
            {
                return TEMPLATE.Render(model);
            }
            
        }

        private TypeModel CreateModel(SheetRawData data, string nameSpace)
        {
            var properties = new List<PropertyModel>();
            _namespaceChain.Clear();
            for (int i = 1; i < data.Rows[0].Length; i++)
            {
                var fieldData = GetField(data.Rows[0][i]);
                properties.Add(new PropertyModel(){ Name = fieldData.name, Type = fieldData.type});
            }
            
            _namespaceChain.Add(typeof(NativeBinaryReader).Namespace);
            _namespaceChain.Remove(null);
            //properties
            return new TypeModel
            {
                NamespaceName = nameSpace,
                TypeKeyword = data.TypeKeyword,
                TypeName = data.SheetName,
                Properties = properties,
                Usings = _namespaceChain.ToList()
            };
        }
        
        private (string type, string name) GetField(string content)
        {
            var f = content.Split(' ');
            var typeString = f.First();
            var typeName = f.Last();
            if (typeString.Contains("[]")) //관리형배열
            {
                AddNameSpaceChain(typeString.Replace("[]", ""));
                return (typeString, typeName);
            }
            else if (typeString.Contains(">")) //제너릭
            {
                int end = typeString.IndexOf(">", StringComparison.Ordinal);
                int start = typeString.IndexOf("<", StringComparison.Ordinal);
                string targetType = typeString.Substring(start + 1, (end - start)-1);
                var containerType = typeString.Substring(0, start);
                AddNameSpaceChain(containerType+"`1", targetType);
                return (typeString, typeName);
            }
            else
            {
                AddNameSpaceChain(typeString);
                return (typeString, typeName);
            }
        }

        private void AddNameSpaceChain(params string[] typeStrings)
        {
            foreach (var typeString in typeStrings)
            {
                //이 친구만 예외처리
                if (typeString.Contains("NativeArray"))
                {
                    _namespaceChain.Add(typeof(MemoryLabel).Namespace);
                    continue;    
                }
                
                var t = TypeFinder.Find(typeString);
                if (t == null)
                    _missingTypes.Add(typeString);
                else
                    _namespaceChain.Add(t.Namespace);
            }
        }

        private void DebugLog_MissionTypes(SheetRawData rawData)
        {
            string missingList = "";
            foreach (var type in _missingTypes)
                missingList += $"{type} ,";
            Debug.LogError($"Failed Generator {rawData.SheetName} Sheet, TypeNotFound : {missingList}");
        }
    }
    

}