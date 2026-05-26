using System;
using Examples;
using LWSerializer;

namespace SheetData.Generator
{
    public partial class ExampleClass : ILwSerializable
    {
        private string _localizeName;
        private float _time;
        private CustomEnum _property;
        private int[] _arr;
        private CustomEnum[] _properties;

        public string localizeName => _localizeName;
        public float time => _time;
        public CustomEnum property => _property;
        public int[] arr => _arr;
        public CustomEnum[] properties => _properties;

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_localizeName);
            writer.Write(_time);
            writer.Write(_property);
            writer.Write(_arr);
            writer.Write(_properties);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _localizeName);
            reader.Read(out _time);
            reader.Read(out _property);
            reader.Read(out _arr);
            reader.Read(out _properties);
        }
    }
}
