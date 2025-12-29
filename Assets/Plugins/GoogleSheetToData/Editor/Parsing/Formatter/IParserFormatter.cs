using System;
using System.Collections.Generic;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    public abstract class IParserFormatter<T> : IParserFormatter
    {
        public abstract string TypeString { get; }
        public abstract void Write(string content, SheetBinaryWriter writer);
    }
    
    public interface IParserFormatter
    {
        void Write(string content, SheetBinaryWriter writer);
    }

    public static class ParserFormatter
    {
        private static Dictionary<string, IParserFormatter> _formatters = new Dictionary<string, IParserFormatter>();

        public static IParserFormatter GetFormatter(Type dataType, string typeString)
        {
            if (!_formatters.TryGetValue(typeString, out IParserFormatter formatter))
            {
                if (dataType.IsPrimitive)
                {
                    var formatterType = typeof(ParserPrimitiveFormatter<>).MakeGenericType(dataType);
                }
                
                
                
                //_formatters.Add(typeString, formatter = (IParserFormatter)Activator.CreateInstance(formatterType));
            }

            return formatter;
        }
    }
}