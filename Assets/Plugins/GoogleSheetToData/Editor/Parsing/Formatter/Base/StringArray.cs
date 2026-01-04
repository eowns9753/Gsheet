using System.Collections.Generic;

namespace SheetData.Scripts.Parsing
{
    public class StringArray
    {
        public static List<string> Convert(string content)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrWhiteSpace(content))
                return result;
            int depth = 0;
            int startIndex = 0;
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                if (c == '{')
                {
                    depth++;
                }
                else if (c == '}')
                {
                    depth--;
                }
                else if (c == ',' && depth == 0)
                {
                    // 괄호 밖(depth == 0)에서 쉼표를 만났을 때만 분리
                    string segment = content.Substring(startIndex, i - startIndex);
                    result.Add(segment);
                    startIndex = i + 1; // 다음 시작 위치 갱신
                }
            }
            if (startIndex < content.Length)
            {
                result.Add(content.Substring(startIndex));
            }
            else if (content.EndsWith(","))
            {
                result.Add("");
            }
            return result;
        }
    }
}