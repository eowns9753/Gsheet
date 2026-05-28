using System;
using System.Collections.Generic;
using SheetData.IO;
using SheetData;

namespace Generator
{
    public partial class Gsheet
    {
        private static Gsheet _instance;
        public List<ExampleClass> _ExampleClass;
        public Dictionary<string, ExampleStruct> _ExampleStruct;
        public List<CustomTypes> _CustomTypes;
        public Dictionary<string, Localize> _Localize;
        public static List<ExampleClass> ExampleClass => Instance._ExampleClass;
        public static Dictionary<string, ExampleStruct> ExampleStruct => Instance._ExampleStruct;
        public static List<CustomTypes> CustomTypes => Instance._CustomTypes;
        public static Dictionary<string, Localize> Localize => Instance._Localize;

        public static Gsheet Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Gsheet();
                    _instance.Load();
                    SheetDataSettingScriptable.Instance.GsheetReLoadFunc = _instance.Load;
                }
                return _instance;
            }
        }

        private void Load()
        {
            //Dispose
            DisposeMember(_ExampleClass);
            DisposeMember(_ExampleStruct);
            DisposeMember(_CustomTypes);
            DisposeMember(_Localize);

            //Read Gsheet Binary
            SheetBinaryReader reader = SheetBinaryReader.Create(SheetDataSettingScriptable.BinaryFileName);
            if(reader == null)
                return;
            
            //Read Data
            reader.Read(out int sheetCount);
            _ExampleClass = SheetDataHelper.ReadSheet<List<ExampleClass>>(reader);
            _ExampleStruct = SheetDataHelper.ReadSheet<Dictionary<string, ExampleStruct>>(reader);
            _CustomTypes = SheetDataHelper.ReadSheet<List<CustomTypes>>(reader);
            _Localize = SheetDataHelper.ReadSheet<Dictionary<string, Localize>>(reader);
            reader.Dispose();
        }

        private void DisposeMember<K, V>(Dictionary<K, V> dic) where V : IDisposable
        {
            if(dic == null) return;
            foreach (var v in dic)
                v.Value.Dispose();
        }

        private void DisposeMember<V>(List<V> list) where V : IDisposable
        {
            if(list == null) return;
            foreach (var v in list)
                v.Dispose();
        }
    }

    public partial class Gsheet
    {
        public const string UNIT_100 = "unit_100";
        public const string UNIT_101 = "unit_101";
        public const string UNIT_102 = "unit_102";
        public const string UNIT_103 = "unit_103";
        public const string UNIT_104 = "unit_104";
        public const string UNIT_300 = "unit_300";
        public const string UNIT_301 = "unit_301";
        public const string UNIT_302 = "unit_302";
        public const string UI_OK = "ui_ok";
        public const string UI_CANCEL = "ui_cancel";
        public const string UI_YES = "ui_yes";
        public const string UI_NO = "ui_no";
        public const string CLASS_0 = "class_0";
        public const string CLASS_1 = "class_1";
        public const string CLASS_2 = "class_2";
        public const string CLASS_3 = "class_3";
        public const string CLASS_4 = "class_4";
        public const string CLASS_5 = "class_5";
        public const string CLASS_6 = "class_6";
    }
}
