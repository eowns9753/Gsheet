using System.Collections.Generic;

namespace SheetData.Editor.Generator
{
    public class TypeModel
    {
        public string NamespaceName { get; set; }
        public string TypeKeyword { get; set; } // "class" 또는 "struct"
        public string TypeName { get; set; }
        public List<string> Usings { get; set; }
        public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();
    }

    public struct PropertyModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
    
    public class Model
    {
        public const string TemplateText = @"
{{~ for us in usings ~}}
using {{ us }}
{{~ end ~}}

namespace {{ namespace_name }}
{
    public {{ type_keyword }} {{ type_name }}
    {
        {{~ for prop in properties ~}}
        public {{ prop.type }} {{ prop.name }} { get; set; }
        {{~ end ~}}
        public {{ type_name }}() 
        {
        }
    }
}
";
    }
}