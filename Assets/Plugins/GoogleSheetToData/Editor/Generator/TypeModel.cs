using System.Collections.Generic;

namespace SheetData.Editor.Generator
{
    public class TypeModel
    {
        public string NamespaceName { get; set; }
        public string TypeKeyword { get; set; } // "class" 또는 "struct"
        public string TypeName { get; set; }
        public List<string> Usings { get; set; }
        public List<MemberModel> structMembers { get; set; } = new ();
        public List<MemberModel> objectMembers { get; set; } = new ();
        public List<MemberModel> lwSerializableMembers { get; set; } = new ();
        public List<GenericMemberModel> nativeArray { get; set; } = new ();
        public List<GenericMemberModel> nativeRef { get; set; } = new ();
    }
    
    public class MemberModel
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public MemberModel(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }

    public class GenericMemberModel : MemberModel
    {
        public string Generic1 { get; set; }
        public GenericMemberModel(string type, string name, string generic)
        : base(type, name)
        {
            Generic1 = generic;
        }
    }
    
    public class TypeModelTemplate
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
        {{~ for prop in native_array ~}}
        private {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in native_ref ~}}
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
        {{~ for prop in native_array ~}}
        public {{ prop.type }} {{ prop.name }} => _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in native_ref ~}}
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
            {{~ for prop in native_array ~}}
            writer.WriteSpan<{{ prop.generic1 }}>(_{{ prop.name }}.AsSpan());
            {{~ end ~}}
            {{~ for prop in native_ref ~}}
            writer.Write(_{{ prop.name }}.Value);
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
            {{~ for prop in native_array ~}}
            _{{ prop.name }} = new NativeArray<{{ prop.generic1 }}>(reader.PeekSpanLength<{{ prop.generic1 }}>(), Allocator.Persistent);
            reader.ReadSpan(_{{ prop.name }}.AsSpan());
            {{~ end ~}}
            {{~ for prop in native_ref ~}}
            _{{ prop.name }} = new NativeReference<{{ prop.generic1 }}>(Allocator.Persistent);
            _{{ prop.name }}.Value = reader.Read<{{ prop.generic1 }}>();
            {{~ end ~}}
        }
    }
}
";
    }
}