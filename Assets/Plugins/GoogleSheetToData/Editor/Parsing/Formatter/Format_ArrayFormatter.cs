using System;
using System.Collections.Generic;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    /// <summary>
    /// 모든 배열을 포매팅해줌
    /// Type1 - a,b,c,d
    /// Type2 - {a,b},{c,b},{e,f}
    /// </summary>
    public class Format_ArrayFormatter<T> : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer)
        {
            var contents = StringArray.Convert(content);
            var elementParser = ParserFormatter.Get(typeof(T).GetElementType());
            writer.Write(contents.Count);
            for (int i = 0; i < contents.Count; i++)
                elementParser.Write(contents[i], writer);
        }
    }
}