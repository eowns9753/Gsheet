using System;
using System.IO;
using System.Threading.Tasks;
using LWSerializer;
using Unity.Collections.LowLevel.Unsafe;
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

        public static async Task<SheetBinaryReader> CreateAsync(string fileName)
        {
            var path = Path.Combine(Application.dataPath, fileName);
            var bytes = await File.ReadAllBytesAsync(path);
            SheetBinaryReader result = new(bytes);
            return result;
        }
        /// <summary> Resource.Load 를 이용해 바이너리를 불러옵니다 </summary>
        public static SheetBinaryReader Create(string resourceName)
        {
            unsafe
            {
                var textAsset = Resources.Load<TextAsset>(resourceName);
                if (textAsset == null)
                    return null;
                var bytes =  textAsset.GetData<byte>();
                SheetBinaryReader result = new SheetBinaryReader(new IntPtr(bytes.GetUnsafePtr()));
                return result;
            }
        }
    }
}