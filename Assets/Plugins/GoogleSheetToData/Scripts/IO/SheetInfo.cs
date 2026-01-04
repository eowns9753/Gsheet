using System;
using LWSerializer;

namespace SheetData.IO
{
    public struct SheetInfo : ILwSerializable
    {
        private string _sheetName;
        private string _gid;
        private int _dataCount;
        private bool _isDictionary;
        
        public string SheetName => _sheetName;
        public string Gid => _gid;
        public int DataCount => _dataCount;
        public bool IsDictionary => _isDictionary;
        public SheetInfo(string sheetName, string gid)
        {
            _sheetName = sheetName;
            _gid = gid;
            _dataCount = 0;
            _isDictionary = false;
        }

        public SheetInfo UpdateInfo(int rowCount, bool isDictionary)
        {
            _dataCount = rowCount;
            _isDictionary = isDictionary;
            return this;
        }
        
        void ILwSerializable.OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_sheetName);
            writer.Write(_gid);
            writer.Write(_dataCount);
            writer.Write(_isDictionary);
        }

        void ILwSerializable.OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _sheetName);
            reader.Read(out _gid);
            reader.Read(out _dataCount);
            reader.Read(out _isDictionary);
        }
    }
}