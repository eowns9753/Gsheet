using System;
using DefaultNamespace;
using Rui.IO.Serialization;
using MemoryPack;

namespace SheetData.Generator
{
    [MemoryPackable]
    public partial class ExampleClass
    {
        public string localizeName;
        public float time;
        public TestEnum property;
        public int[] arr;
        public TestEnum[] properties;

    }
}
