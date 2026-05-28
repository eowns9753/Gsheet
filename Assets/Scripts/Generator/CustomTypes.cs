using System;
using UnityEngine;
using Examples;
using LWSerializer;

namespace Generator
{
    [System.Serializable]
    public partial class CustomTypes : ILwSerializable, IDisposable
    {
        [SerializeField] private Color _color;
        [SerializeField] private float _time;
        [SerializeField] private CustomStruct _c_struct;
        [SerializeField] private CustomClass _c_class;
        [SerializeField] private SpecialClass _cs_class;

        public Color color => _color;
        public float time => _time;
        public CustomStruct c_struct => _c_struct;
        public CustomClass c_class => _c_class;
        public SpecialClass cs_class => _cs_class;

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_color);
            writer.Write(_time);
            writer.Write(_c_struct);
            writer.Write(_c_class);
            writer.Write(_cs_class);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _color);
            reader.Read(out _time);
            reader.Read(out _c_struct);
            reader.Read(out _c_class);
            reader.Read(out _cs_class);
        }

        public void Dispose()
        {
        }
    }
}
