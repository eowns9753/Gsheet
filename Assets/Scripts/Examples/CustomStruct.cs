using System;
using LWSerializer;
using SheetData.IO;
using SheetData.Scripts.Parsing;
using Unity.Collections;

namespace Examples
{
    //unmanaged struct
    public struct CustomStruct : IGSheetParser//IComponentData
    {
        public FixedString32Bytes monsterName;
        public CustomEnum property;
        public float speed;

        public object ToData(string content)
        {
            var datas = StringArray.Convert(content);
            CustomStruct result = new();
            result.monsterName = datas[0];
            result.property = Enum.Parse<CustomEnum>(datas[1]);
            result.speed = float.Parse(datas[2]);
            return result;
        }

        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write((CustomStruct)((IParserFormatter)this).ToData(content));
        }

        public void OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(monsterName, property, speed);
        }

        public void OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out monsterName, out property, out speed);
        }
    }
}