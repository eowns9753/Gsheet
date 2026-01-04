using System;
using SheetData.IO;
using Unity.Collections;

namespace SheetData.Scripts.Parsing
{
    [ParserTrigger(typeof(NativeArray<>))]
    public class Format_NativeArray<T> : IParserFormatter where T : unmanaged
    {
        public void Write(string content, SheetBinaryWriter writer)
        {
            var items = StringArray.Convert(content);
            writer.Write(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                ParserFormatter.Get(typeof(T)).Write(items[i], writer);
            }
        }
    }

    [ParserTrigger(typeof(NativeReference<>))]
    public class Format_NativeReference<T> : IParserFormatter where T : unmanaged
    {
        public void Write(string content, SheetBinaryWriter writer)
        {
            ParserFormatter.Get(typeof(T)).Write(content, writer);
        }
    }
    
    [ParserTrigger(typeof(FixedString32Bytes))]
    public class Format_FixedString32 : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer) => 
            writer.Write( new FixedString32Bytes(content));
    }
    
    [ParserTrigger(typeof(FixedString64Bytes))]
    public class Format_FixedString64 : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer) => 
            writer.Write( new FixedString64Bytes(content));
    }
    
    [ParserTrigger(typeof(FixedString128Bytes))]
    public class Format_FixedString128 : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer) => 
            writer.Write( new FixedString128Bytes(content));
    }
    
    [ParserTrigger(typeof(FixedString512Bytes))]
    public class Format_FixedString512 : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer) => 
            writer.Write( new FixedString512Bytes(content));
    }
    
    [ParserTrigger(typeof(FixedString4096Bytes))]
    public class Format_FixedString4096 : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer) => 
            writer.Write( new FixedString4096Bytes(content));
    }

}