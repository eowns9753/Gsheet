using System;
using System.Collections.Generic;
using System.Linq;

namespace Localize.Elements
{
    /// <summary> Enum관련 파싱작업이 빈번 할 때, 캐시해서 GC를 완화시켜줍니다 </summary>
    public class EnumCache<T> where T: struct
    {
        private Dictionary<T, string> _enumToString = new Dictionary<T, string>();
        private Dictionary<string, T> _stringToEnums = new Dictionary<string,T>();
        private List<string> _ignoreStrings = new List<string>();
        private T[] _enumArrays = null;
        
        public string ToString(T val)
        {
            if (!_enumToString.ContainsKey(val))
                _enumToString.Add(val,val.ToString());
            return _enumToString[val];
        }

        public T ToEnum(string str)
        {
            if (_ignoreStrings.Contains(str))
                throw new InvalidCastException(typeof(T).ToString() + "The "+str+" Enum value does not exist in the format.");
            _stringToEnums.TryAdd(str, (T)Enum.Parse(typeof(T),str));
            return _stringToEnums[str];
        }

        public T[] GetEnumArray()
        {
            if (_enumArrays == null)
                _enumArrays = Enum.GetValues(typeof(T)).OfType<T>().ToArray();
            return _enumArrays;
        }

        public bool IsDefined(T enumIndex)
        {
            return Enum.IsDefined(typeof(T), enumIndex);
        }
        
        public bool HasEnum(string str)
        {
            if (_stringToEnums.ContainsKey(str))
                return true;
            if (_ignoreStrings.Contains(str))
                return false;
            
            T result = default;
            if (Enum.TryParse(str, out result))
            {
                ToEnum(str); // Cache
                return true;
            }
            _ignoreStrings.Add(str);
            return false;
        }
    }
}