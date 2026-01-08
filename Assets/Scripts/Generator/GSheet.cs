
using System.Collections.Generic;
using SheetData.IO;

namespace SheetData.Generator
{
    //싱글톤으로 만들어주세요
    public static partial class GSheet
    {
        public static List<ExampleClass> ExampleClass;
        public static Dictionary<string, ExampleSturct> ExampleSturct;
        public static Dictionary<string, Localize> Localize;

        public static void Load()
        {
            SheetBinaryReader reader = SheetBinaryReader.Create(SheetDataSettingScriptable.BinaryFileName);
            reader.Read(out int sheetCount);
            ExampleClass = SheetDataHelper.ReadSheet<List<ExampleClass>>(reader);
            ExampleSturct = SheetDataHelper.ReadSheet<Dictionary<string, ExampleSturct>>(reader);
            Localize = SheetDataHelper.ReadSheet<Dictionary<string, Localize>>(reader);
            reader.Dispose();
        }
    }
}
