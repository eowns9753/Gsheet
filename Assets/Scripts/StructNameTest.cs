using System.IO;
using LWSerializer;
using MemoryPack;

namespace DefaultNamespace
{
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