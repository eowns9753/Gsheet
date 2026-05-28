using System;
using System.Collections.Generic;
using System.Linq;
using LWSerializer;
using Scriban;
using SheetData.Editor.DownLoader;
using SheetData.IO;

namespace SheetData.Editor.Generator
{
    public class TypeModel
    {
        private static readonly Template TEMPLATE = Template.Parse(TypeModelTemplate.Template_Class);
        
        public string NamespaceName { get; set; }
        public string TypeKeyword { get; set; } // "class" 또는 "struct"
        public string TypeName { get; set; }
        public List<string> Usings { get; set; }
        //반드시 Write순서와 동일하게 유지 해 주 세 요
        public List<MemberModel> Members { get; set; } = new ();
        
        public TypeModel(SheetRawData data, string nameSpace)
        {
            HashSet<string> namespaceChain = new();
            foreach (var h in data.Headers)
            {
                namespaceChain.Add(h?.type?.Namespace);
                namespaceChain.Add(h?.genericType?.Namespace);
            }

            for (int i = 1; i < data.Headers.Count; i++)
            {
                var typeData = data.Headers[i];
                Members.Add(new  MemberModel(typeData));
            }

            namespaceChain.Add(typeof(LwBinaryReader).Namespace);
            namespaceChain.Remove(null);
            NamespaceName = nameSpace;
            TypeKeyword = data.TypeKeyword;
            TypeName = data.SheetName;
            Usings = namespaceChain.ToList();
        }
        
        public IEnumerable<MemberModel> AllMembers
        {
            get
            {
                // 반드시 Write 순서와 동일하게 반환합니다.
                foreach (var m in Members) yield return m;
            }
        }

        public string Generator()
        {
            return TEMPLATE.Render(this);
        }
    }
    
    public class TypeModelTemplate
    {
        public const string Template_Class = @"{{~ for us in usings ~}}
using {{ us }};
{{~ end ~}}

namespace {{ namespace_name }}
{
    public partial {{ type_keyword }} {{ type_name }} : ILwSerializable, IDisposable
    {
        {{~ for prop in members ~}}
        private {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}

        {{~ for prop in members ~}}
        public {{ prop.type }} {{ prop.name }} => _{{ prop.name }};
        {{~ end ~}}

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            {{~ for prop in members ~}}
            writer.Write(_{{ prop.name }});
            {{~ end ~}}
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            {{~ for prop in members ~}}
            reader.Read(out _{{ prop.name }});
            {{~ end ~}}
        }

        public void Dispose()
        {
            {{~ for prop in members ~}}
            {{~ if prop.header_type.is_disposable ~}}
            {{ prop.name }}.Dispose();
            {{~ end ~}}
            {{~ end ~}}
        }
    }
}
";
    }
}