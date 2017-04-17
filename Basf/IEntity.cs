namespace Basf
{
    public interface IEntity<TKey>
    {
        TKey UniqueId { get; }
    }
}
