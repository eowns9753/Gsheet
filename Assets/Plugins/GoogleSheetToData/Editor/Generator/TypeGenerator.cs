using System;
using System.Collections.Generic;
using System.Linq;
using Rui.IO.Serialization;
using Scriban;
using SheetData.Editor.DownLoader;
using SheetData.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace SheetData.Editor.Generator
{
    internal static class TypeGenerator
    {
        private static readonly Template TEMPLATE = Template.Parse(Model.TemplateText);

        public static void Generator(SheetRawData sheetData, string generatorRootPath, string nameSpace)
        {
            if (!sheetData.IsValidation())
                return;
            var model = CreateModel(sheetData, nameSpace);

            string result = TEMPLATE.Render(model);
            System.IO.File.WriteAllText(IOUtils.GetSystemPath($"{generatorRootPath}/{model.TypeName}.cs"), result);
           
        }

        private static TypeModel CreateModel(SheetRawData data, string nameSpace)
        {
            var properties = new List<PropertyModel>();
            var unsings = new HashSet<string>();
            
            for (int i = 1; i < data.Rows[0].Length; i++)
            {
                var fieldData = GetField(data.Rows[0][i], unsings);
                properties.Add(new PropertyModel(){ Name = fieldData.name, Type = fieldData.type});
            }
            
            unsings.Add(typeof(NativeBinaryReader).Namespace);
            unsings.Remove(null);
            //properties
            return new TypeModel
            {
                NamespaceName = nameSpace,
                TypeKeyword = data.TypeKeyword,
                TypeName = data.SheetName,
                Properties = properties,
                Usings = unsings.ToList()
            };
        }


        private static (string type, string name) GetField(string content, HashSet<string> namespaceChain)
        {
            var f = content.Split(' ');
            var typeString = f.First();
            var typeName = f.Last();
            if (typeString.Contains("[]")) //관리형배열
            {
                namespaceChain.Add(TypeFinder.Find(typeString.Replace("[]", ""))?.Namespace);
                return (typeString, typeName);
            }
            else if (typeString.Contains(">")) //제너릭
            {
                int end = typeString.IndexOf(">", StringComparison.Ordinal);
                int start = typeString.IndexOf("<", StringComparison.Ordinal);
                string targetType = typeString.Substring(start, end - start);
                namespaceChain.Add(TypeFinder.Find(targetType)?.Namespace);
                return (typeString, typeName);
            }
            else
            {
                namespaceChain.Add(TypeFinder.Find(typeString)?.Namespace);
                return (typeString, typeName);
            }
        }
    }
    

}