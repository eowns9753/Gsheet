
using System.Collections.Generic;
using SheetData.IO;

namespace SheetData.Generator
{
    public partial class Gsheet
    {
        private static Gsheet _instance;
        
        public List<ExampleClass> _exampleClass;
        public Dictionary<string, ExampleSturct> _exampleSturct;
        public Dictionary<string, Localize> _localize;
        
        public static List<ExampleClass> ExampleClass => Instance._exampleClass;
        public static Dictionary<string, ExampleSturct> ExampleSturct => Instance._exampleSturct;
        public static Dictionary<string, Localize> Localize => Instance._localize;

        public static Gsheet Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Gsheet();
                    _instance.Load();
                }
                return _instance;
            }
        }

        private void Load()
        {
            SheetBinaryReader reader = SheetBinaryReader.Create(SheetDataSettingScriptable.BinaryFileName);
            reader.Read(out int sheetCount);
            _exampleClass = SheetDataHelper.ReadSheet<List<ExampleClass>>(reader);
            _exampleSturct = SheetDataHelper.ReadSheet<Dictionary<string, ExampleSturct>>(reader);
            _localize = SheetDataHelper.ReadSheet<Dictionary<string, Localize>>(reader);
            reader.Dispose();
        }
    }

}
