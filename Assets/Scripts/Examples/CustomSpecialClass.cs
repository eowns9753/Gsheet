using LWSerializer;
using SheetData.IO;
using SheetData.Scripts.Parsing;
using Unity.Collections;

namespace Examples
{
    /// <summary>
    /// IGSheetParser가 구현된 타입과 실제로 파싱을 하게되는 타입이 다를경우 ParserTrigger 어트리뷰트를 사용해
    /// 대상 타입을 지정할수있습니다.
    /// </summary>
    [ParserTrigger(typeof(SpecialClass))]
    public class SpecialClass : IGSheetParser
    {
        private string _name;
        private float[] _floats;

        public object ToData(string content)
        {
            var datas = StringArray.Convert(content);
            SpecialClass result = new();
            result._name = datas[0];
            result._floats = (float[])ParserFormatter.Get(typeof(float[])).ToData(datas[1]);
            return result;
        }

        public void Write(string content, SheetBinaryWriter writer)
        {
            writer.Write((SpecialClass)((IParserFormatter)this).ToData(content));
        }

        public void OnNativeWrite(LwBinaryWriter writer)
        {
            writer.Write(_name, _floats);
        }

        public void OnNativeRead(LwBinaryReader reader)
        {
            reader.Read(out _name, out _floats);
        }
    }

}