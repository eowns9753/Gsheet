using System;

namespace SheetData.Editor.Generator
{
    public interface ICustomTypeGeneratorable
    {
        Type GetType();
        
    }
}