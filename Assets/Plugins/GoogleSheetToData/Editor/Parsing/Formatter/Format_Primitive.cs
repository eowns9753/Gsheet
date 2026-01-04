using System;
using LWSerializer;
using SheetData.Editor.DownLoader;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    [ParserTrigger(typeof(string))]
    public class Format_String : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write(content);
        }
    }
    
    
    public class Format_Primitive<T> : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write((T)Convert.ChangeType(content, typeof(T)));
        }
    }
}