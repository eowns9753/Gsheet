using System;
using System.IO;
using System.Threading.Tasks;
using LWSerializer;
using UnityEngine;

namespace SheetData.IO
{
    public class SheetBinaryReader : LwBinaryReader
    {
        private SheetBinaryReader(LwNativePointer<byte> span) : base(span)
        { }
        private SheetBinaryReader(IntPtr binaryData) : base(binaryData)
        { }
        private SheetBinaryReader(byte[] binaryData) : base(binaryData)
        { }

        public static async Task<SheetBinaryReader> Create(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            var bytes = await File.ReadAllBytesAsync(path);
            SheetBinaryReader result = new(bytes);
            return result;
        }
    }
}