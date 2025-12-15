using System;
using Rui.IO.Serialization;
using MemoryPack;

namespace SheetData.Generator
{
    [MemoryPackable]
    public partial class Localize
    {
        public string EN { get; private set; }
        public string KR { get; private set; }
        public string JP { get; private set; }
        public string CN { get; private set; }
        public string TW { get; private set; }

    }
}
