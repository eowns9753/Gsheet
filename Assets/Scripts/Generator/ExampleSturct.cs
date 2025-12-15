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
        public Vector2 direction { get; set; }
        public float speed { get; set; }
        public NativeList<int> nativeArr { get; set; }
        public FixedString32Bytes testfxStr { get; set; }
        public FixedString32Bytes testfxStr2 { get; set; }
        public float asdasd { get; set; }

        public string aaaa { get; set; }
        //public StructNameTest a { get; private set; }

    }
}
