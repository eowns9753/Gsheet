using System;
using UnityEngine;
using Unity.Collections;
using LWSerializer;

namespace Generator
{
    [System.Serializable]
    public partial struct ExampleStruct : ILwSerializable, IDisposable
    {
        [SerializeField] private Vector2 _direction;
        [SerializeField] private float _speed;
        [SerializeField] private NativeArray<int> _nativeArr;
        [SerializeField] private FixedString32Bytes _testfxStr;
        [SerializeField] private string _a;
        [SerializeField] private NativeReference<int> _refInt;

        public Vector2 direction => _direction;
        public float speed => _speed;
        public NativeArray<int> nativeArr => _nativeArr;
        public FixedString32Bytes testfxStr => _testfxStr;
        public string a => _a;
        public NativeReference<int> refInt => _refInt;

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_direction);
            writer.Write(_speed);
            writer.Write(_nativeArr);
            writer.Write(_testfxStr);
            writer.Write(_a);
            writer.Write(_refInt);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _direction);
            reader.Read(out _speed);
            reader.Read(out _nativeArr);
            reader.Read(out _testfxStr);
            reader.Read(out _a);
            reader.Read(out _refInt);
        }

        public void Dispose()
        {
            nativeArr.Dispose();
            refInt.Dispose();
        }
    }
}
