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
        public void Write(string content, SheetBinaryWriter writer)
        {
            int.TryParse(content, out var result);
            writer.Write(result);
        }
    }
    
    [MemoryPackable]
    public partial class StructNameTest : ILwSerializable
    {
        private int a;

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