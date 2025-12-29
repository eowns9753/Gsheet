using System;
using System.Collections;
using System.Collections.Generic;
using LWSerializer;

namespace SheetData.Scripts.Parsing
{
    public class DicParser : ISheetParserable<List<int>>
    {
        public override void ParseAndWrite(string strData, LwBinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
    
    public class DicfloatParser : ISheetParserable<List<float>>
    {
        public override void ParseAndWrite(string strData, LwBinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
    
    public abstract class ISheetParserable<T> : ISheetParserable
    {
        
    }
    
    public abstract class ISheetParserable
    {
        public abstract void ParseAndWrite(string strData, LwBinaryWriter writer);
    }
}