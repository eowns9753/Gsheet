using System;
using SheetData.Editor.Generator;

namespace SheetData.Editor.DownLoader
{
    public class HeaderType
    {
        public string originalText { get; private set; }
        public Type type { get; private set; }
        public Type genericType { get; private set; }

        public HeaderType(string headerText)
        {
            originalText = headerText;
            if (headerText.Contains('<'))
            {
                var generic = TypeFinder.GetGenericType(headerText);
                type = TypeFinder.Find(generic.containerType);
                genericType = TypeFinder.Find(generic.genericType);
            }
            else
            {
                type = TypeFinder.Find(headerText);
            }
        }
    }
}