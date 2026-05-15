using System;
using System.Collections;
using System.Collections.Generic;
using LWSerializer;

namespace SheetData.IO
{
    public static class SheetDataHelper
    {
        //WriteDirect 참조
        public static object ReadSheet(SheetBinaryReader reader)
        {
            List<object> list = new();
            Dictionary<string, ILwSerializable> dic = new();
            reader.Read(out SheetInfo info);
            for (int j = 0; j < info.DataCount; j++)
            {
                var instance = (ILwSerializable)Activator.CreateInstance(info.GetSheetType());
                if (info.IsDictionary)
                {
                    reader.Read(out string key);
                    instance.OnNativeRead(reader);
                    dic.Add(key, instance);
                }
                else
                {
                    instance.OnNativeRead(reader);
                    list.Add(instance);
                }
            }
            reader.ReadPadding(32);
            return info.IsDictionary ? dic : list.ToArray();
        }
        
        public static T ReadSheet<T>(SheetBinaryReader reader)
        {
            var sheetResult = ReadSheet(reader);
            var targetType = typeof(T);
            
            if (typeof(IList).IsAssignableFrom(targetType))
            {
                IList result = (IList)Activator.CreateInstance<T>();
                foreach (var itm in (IList)sheetResult)
                    result.Add(itm);
                return (T)result;
            }
            else if (typeof(IDictionary).IsAssignableFrom(targetType))
            {
                IDictionary result = (IDictionary)Activator.CreateInstance<T>();
                foreach (DictionaryEntry key in (IDictionary)sheetResult)
                    result.Add(key.Key, key.Value);
                return (T)result;
            }
            else if (targetType.IsArray)
            {
                return (T)sheetResult;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}