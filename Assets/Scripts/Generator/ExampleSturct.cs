using System;
using UnityEngine;
using Unity.Collections;
using LWSerializer;

namespace SheetData.Generator
{
    public partial struct ExampleSturct : ILwSerializable
    {
        private Vector2 _direction;
        private float _speed;
        private NativeArray<int> _nativeArr;
        private FixedString32Bytes _testfxStr;
        private string _a;
        private NativeReference<int> _refInt;

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
    }
}
