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
        
        
        public string Generator(SheetRawData sheetData, string nameSpace)
        {
            if (!sheetData.IsValidation())
                return "";
            var model = CreateModel(sheetData, nameSpace);
            return TEMPLATE.Render(model);
        }

        private TypeModel CreateModel(SheetRawData data, string nameSpace)
        {
            var m_nativeArray = new List<GenericMemberModel>();
            var m_nativeRef = new List<GenericMemberModel>();
            var m_struct = new List<MemberModel>();
            var m_lwSerializable = new List<MemberModel>();
            var m_array = new List<MemberModel>();
            
            HashSet<string> namespaceChain = new();
            foreach (var h in data.Headers)
            {
                namespaceChain.Add(h?.type?.Namespace);
                namespaceChain.Add(h?.genericType?.Namespace);
            }

            for (int i = 1; i < data.Headers.Count; i++)
            {
                var typeData = data.Headers[i];
                if (typeData.typeString.Contains("NativeArray"))
                {
                    m_nativeArray.Add(new GenericMemberModel(typeData));
                    continue;
                }
                if (typeData.typeString.Contains("NativeReference"))
                {
                    m_nativeRef.Add(new GenericMemberModel(typeData));
                    continue;
                }
                if (typeData.typeString == "string")
                {
                    m_struct.Add(new MemberModel(typeData));
                    continue;
                }
                if (typeData.IsUnmanaged)
                {
                    m_struct.Add(new MemberModel(typeData));
                    continue;
                }
                if (typeData.IsLwSerializable)
                {
                    m_lwSerializable.Add(new MemberModel(typeData));
                    continue;
                }
                if (typeData.IsArray)
                {
                    m_array.Add(new MemberModel(typeData));
                    continue;
                }
                
                throw new Exception($"{typeData.typeString} The ILwSerializable interface is not implemented for this type." +
                                    $" Only unmanaged types or data with the ILwSerializable interface implemented are allowed.");
            }
            namespaceChain.Add(typeof(LwBinaryReader).Namespace);
            namespaceChain.Remove(null);
            //properties
            return new TypeModel
            {
                NamespaceName = nameSpace,
                TypeKeyword = data.TypeKeyword,
                TypeName = data.SheetName,
                arrayMembers = m_array,
                structMembers = m_struct,
                lwSerializableMembers = m_lwSerializable,
                nativeArray = m_nativeArray,
                nativeRef = m_nativeRef,
                Usings = namespaceChain.ToList()
            };
        }

        /*private void AddNameSpaceChain(params string[] typeStrings)
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
        }*/
    }
    

}