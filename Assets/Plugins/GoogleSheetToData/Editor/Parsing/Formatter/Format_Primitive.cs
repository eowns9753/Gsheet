using System;
using LWSerializer;
using SheetData.Editor.DownLoader;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    public abstract class Format_Primitive<T> : ParserFormatterBase<T>
    {
        public override void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            writer.Write((T)FromString(contentType, content));
        }

        public override object FromString(Type contentType, string content)
        {
            return Convert.ChangeType(content, typeof(T));
        }
    }
    
    public abstract class Format_PrimitiveArray<T> : ParserFormatterBase<T[]>
    {
        public override void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            writer.Write((T[])FromString(contentType, content));
        }

        public override object FromString(Type contentType, string content)
        {
            var arr = ToSplited(content);
            T[] result = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                object obj = null;
                if (arr[i] == "")
                    obj = default(T); 
                else
                    obj = ParserFormatter.Get(typeof(T)).FromString(contentType, arr[i]);
                result[i] = (T)obj;
            }
            return result;
        }
    }
    public class FormatPrimitiveString : Format_Primitive<string> { }
    public class FormatPrimitiveChar : Format_Primitive<char> { }
    public class FormatPrimitiveBool : Format_Primitive<bool> { }
    
    public class FormatPrimitiveByte : Format_Primitive<byte> { }
    public class FormatPrimitiveShort : Format_Primitive<short> { }
    public class FormatPrimitiveInt : Format_Primitive<int> { }
    public class FormatPrimitiveLong : Format_Primitive<long> { }
    public class FormatPrimitiveDecimal : Format_Primitive<decimal> { }
    
    public class FormatPrimitiveFloat : Format_Primitive<float> { }
    public class FormatPrimitiveDouble : Format_Primitive<double> { }
    
    //Array Start
    public class FormatPrimitiveStringArray : Format_PrimitiveArray<string> { }
    public class FormatPrimitiveCharArray : Format_PrimitiveArray<char> { }
    public class FormatPrimitiveBoolArray : Format_PrimitiveArray<bool> { }
    
    public class FormatPrimitiveByteArray : Format_PrimitiveArray<byte> { }
    public class FormatPrimitiveShortArray : Format_PrimitiveArray<short> { }
    public class FormatPrimitiveIntArray : Format_PrimitiveArray<int> { }
    public class FormatPrimitiveLongArray : Format_PrimitiveArray<long> { }
    public class FormatPrimitiveDecimalArray : Format_PrimitiveArray<decimal> { }
    
    public class FormatPrimitiveFloatArray : Format_PrimitiveArray<float> { }
    public class FormatPrimitiveDoubleArray : Format_PrimitiveArray<double> { }
    
    //public class Primitive_byte : Primitive<byte> { }
    

}