using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SheetData.Editor.DownLoader
{
    public class SheetRawData
    {
        delegate void SplitRowStringForEachHandler(string str, int index);
        private int _columnCount;
        private string _sheetName;
        private List<string[]> _rows;
        
        public string SheetName => _sheetName;
        public List<string[]> Rows => _rows;
        public string TypeKeyword => _rows.First().First().ToLower();
        
        public SheetRawData(string sheetName, string csvData)
        {
            _sheetName = sheetName;
            if (string.IsNullOrEmpty(csvData))
                return;
            _rows = new();
            List<int> ignoreIdxList = new List<int>();
            var rows = csvData.Split("\r\n");
            var length = SearchIgnoreColumns(rows[0], ignoreIdxList);
            foreach (var row in rows)
            {
                string[] strs = new string[length];
                if(IsAnnotation(row))
                    continue;
                int idx = 0;
                SplitRowString(row, (str, index) =>
                {
                    if (!ignoreIdxList.Contains(index))
                        strs[idx++] = str;
                });
                _rows.Add(strs);
            }
        }

        public bool IsValidation()
        {
            if (_rows.Count < 2)
            {
                Debug.LogError($"{SheetName}' row more than once");
                return false;
            }
            if (_rows[0].Length < 3)
            {
                Debug.LogError($"{SheetName}' column more than two");
                return false;
            }

            return true;
        }
        
        bool IsAnnotation(string strs) => strs.Length >= 2 && strs.Substring(0, 2) == "//";

        int SearchIgnoreColumns(string firstRow, List<int> ignoreColumIdxs)
        {
            var length = 0;
            SplitRowString(firstRow, (str, index) =>
            {
                if (IsAnnotation(str))
                    ignoreColumIdxs.Add(index);
                else
                    length++;
            });
            return length;
        }
        
        void SplitRowString(string row, SplitRowStringForEachHandler caller)
        {
            int splitIdx = 0;
            int startIdx = 0;
            int len = 0;
            bool strOpenState = false;
            for (int i = 0; i < row.Length; i++)
            {
                if (!strOpenState && row[i] == ',')
                {
                    caller.Invoke(len == 0 ? "" : row.Substring(startIdx, len), splitIdx++);
                    startIdx = i + 1;
                    len = 0;
                }
                else
                {
                    if (row[i] == '"')
                    {
                        if (!strOpenState)
                            startIdx++;
                        strOpenState = !strOpenState;
                    }
                    else
                        len++;
                }
            }
            caller.Invoke(len == 0 ? "" : row.Substring(startIdx, len), splitIdx++);
        }
        
        
    }
}