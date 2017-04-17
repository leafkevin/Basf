namespace Basf.Domain
{
    public interface IAggRootState<TKey>
    {
        TKey UniqueId { get; }
        int Version { get; }
    }
}
