namespace api.config.importer
{
    public interface IRow
    {
        int Length { get; }
        object this[int index] { get; }
    }
}
