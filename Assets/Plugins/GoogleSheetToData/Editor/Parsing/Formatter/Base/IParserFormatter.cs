using System;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParserTriggerAttribute : Attribute
    {
        public Type TriggerType { get; }
        public ParserTriggerAttribute(Type targetType) => TriggerType = targetType;
    }
    
    public interface IParserFormatter
    {
        public object ToData(string content);
        public void Write(string content, SheetBinaryWriter writer);
    }
}