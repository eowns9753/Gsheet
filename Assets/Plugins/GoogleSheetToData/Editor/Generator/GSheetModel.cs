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
        
        public GSheetModel(SheetRawData[] datas, string namespaceName)
        {
            this.NamespaceName = namespaceName;
            foreach (var data in datas)
            {
                Members.Add(new MemberModel(data.SheetName, 
                    data.IsDictionary() ? $"Dictionary<string, {data.SheetName}>" :
                        $"List<{data.SheetName}>"));
            }
        }
        public string Generator()
        {
            return TEMPLATE.Render(this);
        }
    }
    
    public class SheetDataTemplate
    {
        public const string Template_Class = @"using System.Collections.Generic;
using SheetData.IO;

namespace {{ namespace_name }}
{
    public partial class {{ class_name }}
    {
        private static {{ class_name }} _instance;
        {{~ for prop in members ~}}
        public {{ prop.type }} _{{ prop.name }};
        {{~ end ~}}
        {{~ for prop in members ~}}
        public static {{ prop.type }} => Instance._{{ prop.name }};
        {{~ end ~}}

        public {{ class_name }} Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new {{ class_name }}();
                    _instance.Load();
                }
                return _instance;
            }
        }

        private void Load()
        {
            SheetBinaryReader reader = SheetBinaryReader.Create(SheetDataSettingScriptable.BinaryFileName);
            reader.Read(out int sheetCount);
            {{~ for prop in members ~}}
            {{ prop.name }} = SheetDataHelper.ReadSheet<{{ prop.type }}>(reader);
            {{~ end ~}}
            reader.Dispose();
        }
    }
}
";
    }
}