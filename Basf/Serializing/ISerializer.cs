using System;

namespace Basf.Serializing
{
    public interface ISerializer<T>
    {
        T Serialize(object obj);
        object Deserialize(T value, Type type);
        TObject Deserialize<TObject>(T value) where TObject : class;
    }
}
