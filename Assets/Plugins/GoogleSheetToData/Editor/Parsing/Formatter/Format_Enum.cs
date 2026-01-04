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
                writer.Write((int)-1);
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
            object[] result = new object[arrs.Length];
            for (int i = 0; i < arrs.Length; i++)
            {
                var elementType = contentType.GetElementType();
                var parsedItem = ParserFormatter.Get(elementType).FromString(elementType, arrs[i]);
                result[i] = parsedItem;
            }
            return result;
        }

        public void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            var result = (object[])FromString(contentType, content);
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == null)
                    writer.Write((int)-1);
                else
                    writer.Write((int)result[i]);
            }
        }
    }
}