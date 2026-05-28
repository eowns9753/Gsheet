using System;
using LWSerializer;
using UnityEngine;

namespace Generator
{
    [System.Serializable]
    public partial class Localize : ILwSerializable, IDisposable
    {
        [SerializeField] private string _EN;
        [SerializeField] private string _KR;
        [SerializeField] private string _JP;
        [SerializeField] private string _CN;
        [SerializeField] private string _TW;

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
