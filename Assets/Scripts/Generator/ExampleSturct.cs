using UnityEngine;
using System;
using Unity.Collections;
using DefaultNamespace;
using LWSerializer;

namespace SheetData.Generator
{
    public partial struct ExampleSturct : ILwSerializable
    {
        internal Vector2 _direction;
        internal float _speed;
        internal FixedString32Bytes _testfxStr;
        internal StructNameTest _a;
        internal NativeList<int> _nativeArr;

        public Vector2 direction => _direction;
        public float speed => _speed;
        public FixedString32Bytes testfxStr => _testfxStr;
        public StructNameTest a => _a;
        public NativeList<int> nativeArr => _nativeArr;

        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_direction);
            writer.Write(_speed);
            writer.Write(_testfxStr);
            writer.WriteRef(_a);
            //writer.Write(_nativeArr);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _direction);
            reader.Read(out _speed);
            reader.Read(out _testfxStr);
            reader.ReadRef(_a);
            //reader.ReadRef(_nativeArr);
        }
    }
}
