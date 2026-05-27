using SheetData.Localize;

namespace SheetData.Localize.Elements
{
    public enum RefreshMode
    {
        Lang,
        Device,
        All,
    }
    public interface ILocalizeListener
    {
        void RefreshLocalize(LocalizeManager manager,RefreshMode mode);
        
    }
}