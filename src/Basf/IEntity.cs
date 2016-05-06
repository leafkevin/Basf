namespace Basf
{
    public interface IEntity
    {
        string UniqueId { get; }
    }
    public interface IEntity<TUniqueId>
    {
        TUniqueId UniqueId { get; }
    }
}
