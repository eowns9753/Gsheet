using System;
using System.Globalization;
using UnityEngine;
using LWSerializer;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    
    public static class VectorParserUtils
    {
        public static Vector2 ParseVector2(ReadOnlySpan<char> span)
        {
            span = span.Trim(new char[] { '{', '}', '(', ')' });
            int commaIndex = span.IndexOf(',');
            float x = float.Parse(span.Slice(0, commaIndex), NumberStyles.Float, CultureInfo.InvariantCulture);
            float y = float.Parse(span.Slice(commaIndex + 1), NumberStyles.Float, CultureInfo.InvariantCulture);
            return new Vector2(x, y);
        }

        public static Vector2Int ParseVector2Int(ReadOnlySpan<char> span)
        {
            span = span.Trim(new char[] { '{', '}', '(', ')' });
            
            int commaIndex = span.IndexOf(',');
            int x = int.Parse(span.Slice(0, commaIndex), NumberStyles.Integer, CultureInfo.InvariantCulture);
            int y = int.Parse(span.Slice(commaIndex + 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
            return new Vector2Int(x, y);
        }

        public static Vector3 ParseVector3(ReadOnlySpan<char> span)
        {
            span = span.Trim(new char[] { '{', '}', '(', ')' });
            
            int firstComma = span.IndexOf(',');
            float x = float.Parse(span.Slice(0, firstComma), NumberStyles.Float, CultureInfo.InvariantCulture);

            ReadOnlySpan<char> remaining = span.Slice(firstComma + 1);
            int secondComma = remaining.IndexOf(',');
            float y = float.Parse(remaining.Slice(0, secondComma), NumberStyles.Float, CultureInfo.InvariantCulture);
            float z = float.Parse(remaining.Slice(secondComma + 1), NumberStyles.Float, CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }

        public static Vector3Int ParseVector3Int(ReadOnlySpan<char> span)
        {
            span = span.Trim(new char[] { '{', '}', '(', ')' });
            
            int firstComma = span.IndexOf(',');
            int x = int.Parse(span.Slice(0, firstComma), NumberStyles.Integer, CultureInfo.InvariantCulture);

            ReadOnlySpan<char> remaining = span.Slice(firstComma + 1);
            int secondComma = remaining.IndexOf(',');
            int y = int.Parse(remaining.Slice(0, secondComma), NumberStyles.Integer, CultureInfo.InvariantCulture);
            int z = int.Parse(remaining.Slice(secondComma + 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
            return new Vector3Int(x, y, z);
        }
    }

    // ==========================================
    // 단일 객체 파서 (Utils 호출로 간소화)
    // ==========================================
    public class Format_Vector2 : ParserFormatterBase<Vector2>
    {
        public override void Write(Type contentType, string content, SheetBinaryWriter writer) => writer.Write((Vector2)FromString(contentType, content));
        public override object FromString(Type contentType, string content) => string.IsNullOrEmpty(content) ? default : VectorParserUtils.ParseVector2(content.AsSpan());
    }

    public class Format_Vector2Int : ParserFormatterBase<Vector2Int>
    {
        public override void Write(Type contentType, string content, SheetBinaryWriter writer) => writer.Write((Vector2Int)FromString(contentType, content));
        public override object FromString(Type contentType, string content) => string.IsNullOrEmpty(content) ? default : VectorParserUtils.ParseVector2Int(content.AsSpan());
    }

    public class Format_Vector3 : ParserFormatterBase<Vector3>
    {
        public override void Write(Type contentType, string content, SheetBinaryWriter writer) => writer.Write((Vector3)FromString(contentType, content));
        public override object FromString(Type contentType, string content) => string.IsNullOrEmpty(content) ? default : VectorParserUtils.ParseVector3(content.AsSpan());
    }

    public class Format_Vector3Int : ParserFormatterBase<Vector3Int>
    {
        public override void Write(Type contentType, string content, SheetBinaryWriter writer) => writer.Write((Vector3Int)FromString(contentType, content));
        public override object FromString(Type contentType, string content) => string.IsNullOrEmpty(content) ? default : VectorParserUtils.ParseVector3Int(content.AsSpan());
    }

    // ==========================================
    // 배열 파서 (Span 슬라이싱으로 Zero Allocation 구현)
    // ==========================================
    public abstract class Format_VectorArrayBase<T> : ParserFormatterBase<T[]>
    {
        protected abstract T ParseItem(ReadOnlySpan<char> span);

        public override void Write(Type contentType, string content, SheetBinaryWriter writer)
        {
            // 배열 쓰기 로직 (필요시 구현)
             writer.Write((T[])FromString(contentType, content));
        }

        public override object FromString(Type contentType, string content)
        {
            if (string.IsNullOrEmpty(content)) return Array.Empty<T>();

            ReadOnlySpan<char> span = content.AsSpan();
            
            // 1. 배열 크기 계산 ( '{' 개수로 추정)
            int count = 0;
            foreach (char c in span)
            {
                if (c == '{') count++;
            }

            T[] result = new T[count];
            int arrayIndex = 0;
            int currentPos = 0;

            // 2. 루프를 돌며 '{'와 '}' 사이를 발라냄
            while (arrayIndex < count && currentPos < span.Length)
            {
                // 현재 위치에서 다음 '{' 찾기
                ReadOnlySpan<char> remaining = span.Slice(currentPos);
                int openBrace = remaining.IndexOf('{');
                if (openBrace == -1) break;

                // '{' 이후에서 '}' 찾기
                int closeBrace = remaining.Slice(openBrace).IndexOf('}');
                if (closeBrace == -1) break;

                // 내용물 추출 (예: "2,1") - 괄호 제외
                // openBrace + 1 위치부터, (closeBrace - 1) 길이만큼
                ReadOnlySpan<char> itemSpan = remaining.Slice(openBrace + 1, closeBrace - 1);
                
                // 파싱 수행
                result[arrayIndex] = ParseItem(itemSpan);

                // 다음 검색 위치 갱신 ('}' 다음부터)
                currentPos += openBrace + closeBrace + 1;
                arrayIndex++;
            }

            return result;
        }
    }

    // 실제 구현 클래스들
    public class Format_Vector2Array : Format_VectorArrayBase<Vector2>
    {
        protected override Vector2 ParseItem(ReadOnlySpan<char> span) => VectorParserUtils.ParseVector2(span);
    }

    public class Format_Vector2IntArray : Format_VectorArrayBase<Vector2Int>
    {
        protected override Vector2Int ParseItem(ReadOnlySpan<char> span) => VectorParserUtils.ParseVector2Int(span);
    }

    public class Format_Vector3Array : Format_VectorArrayBase<Vector3>
    {
        protected override Vector3 ParseItem(ReadOnlySpan<char> span) => VectorParserUtils.ParseVector3(span);
    }

    public class Format_Vector3IntArray : Format_VectorArrayBase<Vector3Int>
    {
        protected override Vector3Int ParseItem(ReadOnlySpan<char> span) => VectorParserUtils.ParseVector3Int(span);
    }
}