namespace api.config.importer
{
    public interface ISheet
    {
        string Name { get; }
        int Length { get; }
        IRow this[int index] { get; }
    }
}
