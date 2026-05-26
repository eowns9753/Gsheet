using System;
using LWSerializer;
using SheetData.IO;
using SheetData.Scripts.Parsing;

namespace Examples
{
    public class CustomClass : IGSheetParser
    {
        private string _unitName;
        private int _unitNumber;
        
        public object ToData(string content)
        {
            var datas = StringArray.Convert(content);
            CustomClass result = new();
            result._unitName = datas[0];
            result._unitNumber = int.Parse(datas[1]);
            return result;
        }

        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write((CustomClass)((IParserFormatter)this).ToData(content));
        }

        public void OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_unitName, _unitNumber);
        }

        public void OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _unitName, out _unitNumber);
        }
    }
}