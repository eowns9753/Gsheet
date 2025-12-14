namespace SheetData.Scripts.Parser
{
    public interface ISheetPaserable<T>
    {
        T Parser(string content);
    }
}