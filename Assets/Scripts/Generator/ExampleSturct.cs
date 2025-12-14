using UnityEngine;
using System;
using Unity.Collections;
using DefaultNamespace;
using Rui.IO.Serialization;
using MemoryPack;

namespace SheetData.Generator
{
    [MemoryPackable]
    public partial struct ExampleSturct
    {
        public Vector2 direction;
        public float speed;
        public NativeList<int> nativeArr;
        public FixedString32Bytes testfxStr;
        public StructNameTest a;

    }
}
