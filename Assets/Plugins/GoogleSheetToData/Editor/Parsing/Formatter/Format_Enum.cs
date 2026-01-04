using System;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    public class Format_Enum<T> : IParserFormatter where T : Enum
    {
        public void Write(string content, SheetBinaryWriter writer)
        {
            Enum.TryParse(typeof(T), content, out var enumValue);
            if (enumValue == null)
                writer.Write(default(T));
            else
                writer.Write((T)enumValue);
        }
    }
}