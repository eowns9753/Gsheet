using System;
using System.Collections.Generic;
using SheetData.Editor.DownLoader;
using SheetData.Editor.Generator;
using SheetData.IO;
using UnityEditor;

namespace SheetData.Scripts.Parsing
{
    
    [InitializeOnLoad]
    public static class ParserFormatter
    {
        private static Dictionary<string, ___IParserFormatter> _parsers;
        private static Format_Enum _enumFormat = new();
        
        static ParserFormatter()
        {
            RefreshParser();
        }
        

        public static ___IParserFormatter Get(Type type)
        {
            ___IParserFormatter formatter = null;
            
            if (type.IsEnum)
            {
                formatter = _enumFormat;
            }
            else
            {
                _parsers.TryGetValue(type.Name, out formatter);
            }
            if(formatter == null)
                throw new Exception($"{type.Name} formatter not found");
            return formatter;
        }

        public static void AddManual<T>(ParserFormatterBase<T> formatter) => AddManual((___IParserFormatter)formatter);
        
        private static void AddManual(___IParserFormatter formatter)
        {
            _parsers.Add(formatter.TypeName, formatter);
        }
        
        public static void RefreshParser()
        {
            _parsers = new();
            var types = TypeFinder.GetAssignableFroms<___IParserFormatter>();
            foreach (var formatterType in types)
            {
                var formatter = (___IParserFormatter)Activator.CreateInstance(formatterType);
                AddManual(formatter);
            }
            //HeaderType  
        }
    }

    public abstract class ParserFormatterBase<T> : ___IParserFormatter
    {
        public string TypeName => typeof(T).Name;
        public abstract object FromString(Type contentType, string content);

        public abstract void Write(Type contentType, string content, SheetBinaryWriter writer);
        public string[] ToSplited(string content)
        {
            return content.Split(',');
        }
    }
    
    public interface ___IParserFormatter
    {
        public string TypeName { get; }
        public object FromString(Type contentType, string content);
        public void Write(Type contentType, string content, SheetBinaryWriter writer);
        
    }
}