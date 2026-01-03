using System;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    public class Format_Enum : ___IParserFormatter
    {
        public string TypeName => "Enum";
        public object FromString(Type contentType, string content)
        {
            Enum.TryParse(contentType, content, out object result);
            return result;
        }

        public void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            var result = FromString(contentType, content);
            if (result == null)
                writer.Write((int)0);
            else
                writer.Write((int)result);
        }
    }
    
    public class Format_EnumArray : ___IParserFormatter
    {
        public string TypeName => "Enum[]";
        public object FromString(Type contentType, string content)
        {
            var arrs = content.Split(',');
            
            Enum.TryParse(contentType, content, out object result);
            return result;
        }

        public void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            var result = FromString(contentType, content);
            if (result == null)
                writer.Write((int)0);
            else
                writer.Write((int)result);
        }
    }
}