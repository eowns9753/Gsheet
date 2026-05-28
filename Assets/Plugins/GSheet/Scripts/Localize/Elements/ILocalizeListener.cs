using SheetData.Localize;

namespace Localize.Elements
{
    public enum RefreshMode
    {
        Lang,
        Device,
        All,
    }
    public interface ILocalizeListener
    {
        void RefreshLocalize(RefreshMode mode);
        
    }
}