using System;
using Examples;
using LWSerializer;
using UnityEngine;

namespace Generator
{
    [System.Serializable]
    public partial class ExampleClass : ILwSerializable, IDisposable
    {
        [SerializeField] private string _localizeName;
        [SerializeField] private float _time;
        [SerializeField] private CustomEnum _property;
        [SerializeField] private int[] _arr;
        [SerializeField] private CustomEnum[] _properties;

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

        public void Dispose()
        {
        }
    }
}
