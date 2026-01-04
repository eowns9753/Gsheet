using System;
using SheetData.IO;
using Unity.Collections;

namespace SheetData.Scripts.Parsing
{
    public class Format_NativeArray<T> : ParserFormatterBase<NativeArray<T>>
        where T : unmanaged
    {
        public override object FromString(Type contentType, string content)
        {
            var contents = ToSplited(content);
            NativeArray<T> array = new NativeArray<T>(contents.Length, Allocator.Temp);
            for (int i = 0; i < contents.Length; i++)
            {
                var elementType = contentType.GetGenericArguments()[0];
                var data = ParserFormatter.Get(elementType).FromString(elementType, contents[i]);
                array[i] = (T)data;
            }
            return array;
        }

        public override void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            var data = FromString(contentType, content);
            writer.Write((NativeArray<T>)data);
        }
    }
    
    public class Format_NativeReference<T> : ParserFormatterBase<NativeReference<T>>
        where T : unmanaged
    {
        public override object FromString(Type contentType, string content)
        {
            NativeReference<T> reference = new NativeReference<T>();
            var elementType = contentType.GetGenericArguments()[0];
            reference.Value = (T)ParserFormatter.Get(elementType).FromString(elementType, content);
            return reference;
        }

        public override void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            var data = FromString(contentType, content);
            writer.Write((NativeReference<T>)data);
        }
    }

}