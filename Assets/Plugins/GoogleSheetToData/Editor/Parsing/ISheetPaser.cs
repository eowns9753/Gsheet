using System;
using System.Collections;
using LWSerializer;

namespace SheetData.Scripts.Parsing
{
    public abstract class ISheetParserable<T> : ISheetParserable
    {
        
    }
    
    public abstract class ISheetParserable
    {
        public abstract void ParseAndWrite(string strData, LwBinaryWriter writer);
    }
}