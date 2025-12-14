using System;
using UnityEngine;

namespace SheetData.Scripts.Parser.Default
{
    public abstract class P_Default<T> : ISheetPaserable<T>
    {
        public abstract T Parser(string content);
        protected void WriteError(string content)
        {
            Debug.Log($"failed Parsing-{content}->{typeof(T).Name}");
        }
    }

    public class P_string : P_Default<string>
    {
        public override string Parser(string content) => content;
    }

    public class P_int : P_Default<int>
    {
        public override int Parser(string content)
        {
            if(!int.TryParse(content, out int result))
                WriteError(content);
            return result;
        }
    }
    
    public class P_float : P_Default<float>
    {
        public override float Parser(string content)
        {
            if (!float.TryParse(content, out float result))
                WriteError(content);
            return result;
        }
    }

    /*
    public class P_Vector2 : P_Default<Vector2>
    {
        public override Vector2 Parser(string content)
        {
            
        }
    }*/
}