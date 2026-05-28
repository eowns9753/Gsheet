using System;
using LWSerializer;

namespace Generator
{
    public partial class Localize : ILwSerializable, IDisposable
    {
        private string _EN;
        private string _KR;
        private string _JP;
        private string _CN;
        private string _TW;

        public string EN => _EN;
        public string KR => _KR;
        public string JP => _JP;
        public string CN => _CN;
        public string TW => _TW;

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_EN);
            writer.Write(_KR);
            writer.Write(_JP);
            writer.Write(_CN);
            writer.Write(_TW);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _EN);
            reader.Read(out _KR);
            reader.Read(out _JP);
            reader.Read(out _CN);
            reader.Read(out _TW);
        }

        public void Dispose()
        {
        }
    }
}
