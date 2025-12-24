using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LWSerializer;
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
            var m_struct = new List<PropertyModel>();
            var m_object = new List<PropertyModel>();
            var m_lwSerializable = new List<PropertyModel>();
            var m_nativeCollection = new List<PropertyModel>();
            
            var ilwSerializableType = typeof(ILwSerializable);
            _namespaceChain.Clear();
            for (int i = 1; i < data.Rows[0].Length; i++)
            {
                var fieldData = GetField(data.Rows[0][i]);
                var propertyModel = new PropertyModel() { Name = fieldData.name, Type = fieldData.type };
                bool haslwSerializable = ilwSerializableType.IsAssignableFrom(fieldData.typedata);

                if (fieldData.typedata == null)
                {
                    if (fieldData.type.Contains("NativeList") || fieldData.type.Contains("NativeArray"))
                    {
                        m_nativeCollection.Add(propertyModel);
                        continue;
                    }
                    throw new Exception($"{fieldData.type} is not type");
                }
                if (haslwSerializable)
                {
                    m_lwSerializable.Add(propertyModel);
                    continue;
                }
                if (TypeFinder.IsUnmanaged(fieldData.typedata))
                {
                    m_struct.Add(propertyModel);
                }
                else
                {
                    if(fieldData.typedata == typeof(string))
                        m_struct.Add(propertyModel);
                    else
                        throw new Exception($"{fieldData.typedata} The ILwSerializable interface is not implemented for this type." +
                                            $" Only unmanaged types or data with the ILwSerializable interface implemented are allowed.");
                }
            }
            
            _namespaceChain.Add(typeof(LwBinaryReader).Namespace);
            _namespaceChain.Remove(null);
            //properties
            return new TypeModel
            {
                NamespaceName = nameSpace,
                TypeKeyword = data.TypeKeyword,
                TypeName = data.SheetName,
                objectMembers = m_object,
                structMembers = m_struct,
                lwSerializableMembers = m_lwSerializable,
                nativeCollections = m_nativeCollection,
                Usings = _namespaceChain.ToList()
            };
        }
        
        private (string type, string name, Type typedata) GetField(string content)
        {
            var f = content.Split(' ');
            var typeString = f.First();
            var typeName = f.Last();
            if (typeString.Contains("[]")) //관리형배열
            {
                var findType = typeString.Replace("[]", "");
                AddNameSpaceChain(findType);
                return (typeString, typeName, TypeFinder.Find(findType));
            }
            else if (typeString.Contains(">")) //제너릭
            {
                int end = typeString.IndexOf(">", StringComparison.Ordinal);
                int start = typeString.IndexOf("<", StringComparison.Ordinal);
                string targetType = typeString.Substring(start + 1, (end - start)-1);
                var containerType = typeString.Substring(0, start);
                AddNameSpaceChain(containerType+"`1", targetType);
                return (typeString, typeName,  TypeFinder.Find(containerType));
            }
            else
            {
                AddNameSpaceChain(typeString);
                return (typeString, typeName,  TypeFinder.Find(typeString));
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