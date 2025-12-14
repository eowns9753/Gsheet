using System;
using Rui.IO.Serialization;
using MemoryPack;

namespace SheetData.Generator
{
    [MemoryPackable]
    public partial class Localize
    {
        public string EN;
        public string KR;
        public string JP;
        public string CN;
        public string TW;

    }
}
