using System;
using System.Globalization;
using UnityEngine;
using LWSerializer;
using SheetData.IO;

namespace SheetData.Scripts.Parsing
{
    [ParserTrigger(typeof(Vector2))]
    public class Format_Vector2 : IParserFormatter
    {
        public void Write(string content, SheetBinaryWriter writer)
        {
            var span = content.AsSpan().Trim(new char[] { '{', '}', '(', ')' });
            int commaIndex = span.IndexOf(',');
            float x = float.Parse(span.Slice(0, commaIndex), NumberStyles.Float, CultureInfo.InvariantCulture);
            float y = float.Parse(span.Slice(commaIndex + 1), NumberStyles.Float, CultureInfo.InvariantCulture);
            writer.Write(new Vector2(x, y));
        }
    }
    [ParserTrigger(typeof(Vector3))]
    public class Format_Vector3 : IParserFormatter
    {
        public Type Type => typeof(Vector3);

        public void Write(string content, SheetBinaryWriter writer)
        {
            var span = content.AsSpan().Trim(new char[] { '{', '}', '(', ')' });
            int firstComma = span.IndexOf(',');
            float x = float.Parse(span.Slice(0, firstComma), NumberStyles.Float, CultureInfo.InvariantCulture);
            ReadOnlySpan<char> remaining = span.Slice(firstComma + 1);
            int secondComma = remaining.IndexOf(',');
            float y = float.Parse(remaining.Slice(0, secondComma), NumberStyles.Float, CultureInfo.InvariantCulture);
            float z = float.Parse(remaining.Slice(secondComma + 1), NumberStyles.Float, CultureInfo.InvariantCulture);
            writer.Write(new Vector3(x, y, z));
        }
    }
    [ParserTrigger(typeof(Vector2Int))]
    public class Format_Vector2Int : IParserFormatter
    {
        public Type Type => typeof(Vector2Int);

        public void Write(string content, SheetBinaryWriter writer)
        {
            var span = content.AsSpan().Trim(new char[] { '{', '}', '(', ')' });
            int commaIndex = span.IndexOf(',');
            int x = int.Parse(span.Slice(0, commaIndex), NumberStyles.Integer, CultureInfo.InvariantCulture);
            int y = int.Parse(span.Slice(commaIndex + 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
            writer.Write(new Vector2Int(x, y));
        }
    }
    [ParserTrigger(typeof(Vector3Int))]
    public class Format_Vector3Int : IParserFormatter
    {
        public Type Type => typeof(Vector3Int);

        public void Write(string content, SheetBinaryWriter writer)
        {
            var span = content.AsSpan().Trim(new char[] { '{', '}', '(', ')' });
            int firstComma = span.IndexOf(',');
            int x = int.Parse(span.Slice(0, firstComma), NumberStyles.Integer, CultureInfo.InvariantCulture);
            ReadOnlySpan<char> remaining = span.Slice(firstComma + 1);
            int secondComma = remaining.IndexOf(',');
            int y = int.Parse(remaining.Slice(0, secondComma), NumberStyles.Integer, CultureInfo.InvariantCulture);
            int z = int.Parse(remaining.Slice(secondComma + 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
            writer.Write(new Vector3Int(x, y, z));
        }
    }
}