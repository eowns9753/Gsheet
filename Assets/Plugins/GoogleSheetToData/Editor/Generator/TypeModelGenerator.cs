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
    internal class TypeModelGenerator
    {
        private static readonly Template TEMPLATE = Template.Parse(TypeModelTemplate.Template_Class);
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
            var m_struct = new List<MemberModel>();
            var m_object = new List<MemberModel>();
            var m_lwSerializable = new List<MemberModel>();
            var m_nativeArray = new List<GenericMemberModel>();
            var m_nativeRef = new List<GenericMemberModel>();
            var ilwSerializableType = typeof(ILwSerializable);
            _namespaceChain.Clear();
            for (int i = 1; i < data.Rows[0].Length; i++)
            {
                var fieldData = GetField(data.Rows[0][i]);
                var propertyModel = new MemberModel(fieldData.type, fieldData.name);
                bool haslwSerializable = ilwSerializableType.IsAssignableFrom(fieldData.typedata);

                if (fieldData.typedata == null)
                {
                    if (fieldData.type.Contains("NativeArray"))
                    {
                        var generic = TypeFinder.GetGenericType(fieldData.type).genericType;
                        m_nativeArray.Add(new GenericMemberModel(fieldData.type, fieldData.name, generic));
                        continue;
                    }
                    if (fieldData.type.Contains("NativeReference"))
                    {
                        var generic = TypeFinder.GetGenericType(fieldData.type).genericType;
                        m_nativeRef.Add(new GenericMemberModel(fieldData.type, fieldData.name, generic));
                        continue;
                    }
                    throw new Exception($"{fieldData.type} is not support type");
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
                nativeArray = m_nativeArray,
                nativeRef = m_nativeRef,
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
                var data = TypeFinder.GetGenericType(typeString);
                AddNameSpaceChain(data.containerType+"`1", data.genericType);
                return (typeString, typeName,  TypeFinder.Find(data.containerType));
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