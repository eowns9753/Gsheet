using System;
using UnityEngine;
using Unity.Collections;
using DefaultNamespace;
using LWSerializer;

namespace SheetData.Generator
{
    public partial struct ExampleSturct : ILwSerializable
    {
        private Vector2 _direction;
        private float _speed;
        private FixedString32Bytes _testfxStr;
        private StructNameTest _a;
        private NativeArray<int> _nativeArr;
        private NativeReference<int> _refInt;

        public Vector2 direction => _direction;
        public float speed => _speed;
        public FixedString32Bytes testfxStr => _testfxStr;
        public StructNameTest a => _a;
        public NativeArray<int> nativeArr => _nativeArr;
        public NativeReference<int> refInt => _refInt;


        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_direction);
            writer.Write(_speed);
            writer.Write(_testfxStr);
            writer.WriteRef(_a);
            writer.WriteSpan<Int32>(_nativeArr.AsSpan());
            writer.Write(_refInt.Value);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _direction);
            reader.Read(out _speed);
            reader.Read(out _testfxStr);
            reader.ReadRef(_a);
            _nativeArr = new NativeArray<Int32>(reader.PeekSpanLength<Int32>(), Allocator.Persistent);
            reader.ReadSpan(_nativeArr.AsSpan());
            _refInt = new NativeReference<Int32>(Allocator.Persistent);
            _refInt.Value = reader.Read<Int32>();
        }
    }
}
