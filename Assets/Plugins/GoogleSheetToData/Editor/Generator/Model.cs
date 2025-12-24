using System.Collections.Generic;

namespace SheetData.Editor.Generator
{
    public class TypeModel
    {
        public string NamespaceName { get; set; }
        public string TypeKeyword { get; set; } // "class" 또는 "struct"
        public string TypeName { get; set; }
        public List<string> Usings { get; set; }
        public List<PropertyModel> structMembers { get; set; } = new List<PropertyModel>();
        public List<PropertyModel> objectMembers { get; set; } = new List<PropertyModel>();
        public List<PropertyModel> lwSerializableMembers { get; set; } = new List<PropertyModel>();
        public List<PropertyModel> nativeCollections { get; set; } = new List<PropertyModel>();
    }
    
    public struct PropertyModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
    
    public class Model
    {
        public const string Template_Class = @"{{~ for us in usings ~}}
using {{ us }};
{{~ end ~}}

namespace {{ namespace_name }}
{
    public partial {{ type_keyword }} {{ type_name }} : ILwSerializable
    {
        {{~ for prop in struct_members ~}}
        private {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in object_members ~}}
        private {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in lw_serializable_members ~}}
        private {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in native_collections ~}}
        private {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}

        {{~ for prop in struct_members ~}}
        public {{ prop.type }} {{ prop.name }} => _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in object_members ~}}
        public {{ prop.type }} {{ prop.name }} => _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in lw_serializable_members ~}}
        public {{ prop.type }} {{ prop.name }} => _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in native_collections ~}}
        public {{ prop.type }} {{ prop.name }} => _{{ prop.name }};
        {{~ end ~}}

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            {{~ for prop in struct_members ~}}
            writer.Write(_{{ prop.name }});
            {{~ end ~}}
            {{~ for prop in object_members ~}}
            writer.WriteRef(_{{ prop.name }});
            {{~ end ~}}
            {{~ for prop in lw_serializable_members ~}}
            writer.WriteRef(_{{ prop.name }});
            {{~ end ~}}
            {{~ for prop in native_collections ~}}
            //writer.Write(_{{ prop.name }});
            {{~ end ~}}
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            {{~ for prop in struct_members ~}}
            reader.Read(out _{{ prop.name }});
            {{~ end ~}}
            {{~ for prop in object_members ~}}
            reader.ReadRef(_{{ prop.name }});
            {{~ end ~}}
            {{~ for prop in lw_serializable_members ~}}
            reader.ReadRef(_{{ prop.name }});
            {{~ end ~}}
            {{~ for prop in native_collections ~}}
            //reader.ReadRef(_{{ prop.name }});
            {{~ end ~}}
        }
    }
}
";
    }
}