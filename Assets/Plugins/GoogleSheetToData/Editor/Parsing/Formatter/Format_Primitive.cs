using System;
using LWSerializer;
using SheetData.Editor.DownLoader;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    [ParserTrigger(typeof(string))]
    public class Format_String : IParserFormatter
    {
        public object ToData(string content)
        {
            return content;
        }

        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write(content);
        }
        
    }
    
    
    public class Format_Primitive<T> : IParserFormatter
    {
        public object ToData(string content)
        {
            return Convert.ChangeType(content, typeof(T));
        }
        
        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write((T)ToData(content));
        }
    }
}