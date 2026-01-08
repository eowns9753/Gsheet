using System.IO;
using LWSerializer;
using MemoryPack;
using SheetData.IO;
using SheetData.Scripts.Parsing;

namespace DefaultNamespace
{
    [ParserTrigger(typeof(StructNameTest))]
    public class Format_StructNameTest : IParserFormatter
    {
        public object ToData(string content)
        {
            int.TryParse(content, out var result);
            StructNameTest data = new();
            data.a = result;
            return data;
        }

        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write((StructNameTest)ToData(content));
        }
    }
    
    [MemoryPackable]
    public partial class StructNameTest : ILwSerializable
    {
        public int a;

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(a);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out a);
        }
    }
}