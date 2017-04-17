namespace Basf.Domain
{
    public interface IAggRoot<TKey>
    {
        TKey UniqueId { get; }
        int Version { get; }
    }
}
