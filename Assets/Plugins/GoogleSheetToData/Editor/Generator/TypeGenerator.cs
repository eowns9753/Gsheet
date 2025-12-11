using System;
using System.Collections.Generic;
using Scriban;
using SheetData.Editor.DownLoader;

namespace SheetData.Editor.Generator
{
    public class TypeGenerator
    {
        private static readonly Template TEMPLATE = Template.Parse(Model.TemplateText);
        public void Generator(SheetRawData sheetData, string path)
        {
            //sheetData   
            var model = CreateModel(sheetData);
            
            // 템플릿 엔진에 데이터 주입
            // 네이밍 컨벤션이 자동으로 매핑됩니다 (NamespaceName -> namespace_name)
            string result = TEMPLATE.Render(model);
            Console.WriteLine($"--- Generated {model.TypeName}.cs ---");
            Console.WriteLine(result);
            Console.WriteLine();
            // 실제로 파일로 저장하려면 아래 주석 해제
            // System.IO.File.WriteAllText($"{model.TypeName}.cs", result);
        }

        private TypeModel CreateModel(SheetRawData data)
        {
            
            /*// 예제 1: 클래스 생성
            new TypeModel 
            { 
                NamespaceName = "Game.Data",
                TypeKeyword = "class", 
                TypeName = "PlayerStats",
                Properties = new List<PropertyModel>
                {
                    new PropertyModel { Type = "int", Name = "Hp" },
                    new PropertyModel { Type = "float", Name = "Speed" },
                    new PropertyModel { Type = "string", Name = "Name" }
                }
            },
            // 예제 2: 구조체 생성
            new TypeModel 
            { 
                NamespaceName = "Game.Core",
                TypeKeyword = "struct", 
                TypeName = "Vector2D",
                Properties = new List<PropertyModel>
                {
                    new PropertyModel { Type = "float", Name = "X" },
                    new PropertyModel { Type = "float", Name = "Y" }
                }
            }*/
            return null;
        }
    }
    

}