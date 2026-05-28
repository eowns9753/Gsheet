using System;
using System.IO;
using System.Threading.Tasks;
using LWSerializer;
using UnityEditor;
using UnityEngine;

namespace SheetData.IO
{
    public class SheetBinaryWriter : LwBinaryWriter
    {
        private string _fileName = "";
        
        private SheetBinaryWriter() : base(1024)
        {
        }

        public static SheetBinaryWriter Create(string filename)
        {
            SheetBinaryWriter result = new();
            result._fileName = filename;
            return result;
        }

        public void Save()
        {
            var path = Path.Combine(Application.dataPath, _fileName);
            ReadOnlySpan<byte> binarySpan = ToPtr().AsSpan(Length);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            try
            {
                using (FileStream fs = File.Create(path, Length))
                {
                    fs.Write(binarySpan);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            AssetDatabase.ImportAsset("Assets/"+_fileName, ImportAssetOptions.ForceSynchronousImport);
        }

    }
}