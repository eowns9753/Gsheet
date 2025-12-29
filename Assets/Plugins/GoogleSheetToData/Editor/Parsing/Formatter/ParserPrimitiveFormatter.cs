using System;
using LWSerializer;
using SheetData.Editor.DownLoader;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
   /*
    * TypeString(string) 
    * Data(string)
    * 유저가 ILwSerializable 구현시 Parser에 대한 대응이필요
    * 그외에는 전부 Dictionary<TypeString, Formatter> 로 가능 
    * ILwSerializable과 별개로
    * IParserFormatter 인터페이스를 통해 구현하고 수동으로 등록
    * 등록과정 간소화 방법
    * SheetData.WriteDirect 작성 전,  Interface가 구현된 어셈블리 를 생성해서 맵에 할당 후
    * 맵을 통해 불러와 활용
    */
    public class ParserPrimitiveFormatter<T> : IParserFormatter<T> where T : unmanaged
    {
        public override string TypeString => "int";
        public override void Write(string content, SheetBinaryWriter writer)
        {
            
        }
    }
}