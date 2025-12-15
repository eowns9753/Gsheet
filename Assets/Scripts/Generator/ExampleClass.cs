using System;
using DefaultNamespace;
using Rui.IO.Serialization;
using MemoryPack;

namespace SheetData.Generator
{
    [MemoryPackable]
    public partial class ExampleClass
    {
        public string localizeName { get; private set; }
        public float time { get; private set; }
        public TestEnum property { get; private set; }
        public int[] arr { get; private set; } = new int[1];
        public TestEnum[] properties { get; private set; } = new TestEnum[1];

    }
}
