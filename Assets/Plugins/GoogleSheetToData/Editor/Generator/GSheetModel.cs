using System.Collections.Generic;
using Scriban;
using SheetData.Editor.DownLoader;

namespace SheetData.Editor.Generator
{
    ////싱글톤으로 만들어주세요
    public class GSheetModel
    {
        private static readonly Template TEMPLATE = Template.Parse(SheetDataTemplate.Template_Class);
        public const string NAME = "Gsheet";
        public string NamespaceName { get; set; }
        public string ClassName => NAME;
        public List<MemberModel> Members { get; set; } = new();
        
        public List<string> DictionaryKeys { get; set; }

        public GSheetModel(SheetRawData[] datas, string namespaceName)
        {
            this.NamespaceName = namespaceName;
            HashSet<string> allKeys = new HashSet<string>();
            DictionaryKeys = new List<string>();
            foreach (var data in datas)
            {
                Members.Add(new MemberModel(data.SheetName, 
                    data.IsDictionary() ? $"Dictionary<string, {data.SheetName}>" :
                        $"List<{data.SheetName}>"));
                for (int i = 1; i < data.Rows.Count; i++)
                    allKeys.Add(data.Rows[i][0]);
            }


            foreach (var key in allKeys)
                if(key.Trim() != string.Empty)
                    DictionaryKeys.Add(key);
            //DictionaryKeys
        }
        public string Generator()
        {
            return TEMPLATE.Render(this);
        }
    }
    
    public class SheetDataTemplate
    {
        public const string Template_Class = @"using System;
using System.Collections.Generic;
using SheetData.IO;
using SheetData;

namespace {{ namespace_name }}
{
    public partial class {{ class_name }}
    {
        private static {{ class_name }} _instance;
        {{~ for prop in members ~}}
        public {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in members ~}}
        public static {{ prop.type }} {{ prop.name }} => Instance._{{ prop.name }};
        {{~ end ~}}

        public static {{ class_name }} Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new {{ class_name }}();
                    _instance.Load();
                    SheetDataSettingScriptable.Instance.GsheetReLoadFunc = _instance.Load;
                }
                return _instance;
            }
        }

        private void Load()
        {
            //Dispose
            {{~ for prop in members ~}}
            DisposeMember(_{{ prop.name }});
            {{~ end ~}}

            //Read Gsheet Binary
            SheetBinaryReader reader = SheetBinaryReader.Create(SheetDataSettingScriptable.BinaryFileName);
            if(reader == null)
                return;
            
            //Read Data
            reader.Read(out int sheetCount);
            {{~ for prop in members ~}}
            _{{ prop.name }} = SheetDataHelper.ReadSheet<{{ prop.type }}>(reader);
            {{~ end ~}}
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
        {{~ for prop in dictionary_keys ~}}
        public const string {{ prop | string.replace '/' '_' | string.upcase }} = ""{{ prop }}"";
        {{~ end ~}}
    }
}
";
    }
}