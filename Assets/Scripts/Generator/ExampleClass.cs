using System;
using DefaultNamespace;
using LWSerializer;

namespace SheetData.Generator
{
    public partial class ExampleClass : ILwSerializable
    {
        internal string _localizeName;
        internal float _time;
        internal TestEnum _property;
        internal int[] _arr;
        internal TestEnum[] _properties;

        public string localizeName => _localizeName;
        public float time => _time;
        public TestEnum property => _property;
        public int[] arr => _arr;
        public TestEnum[] properties => _properties;

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
