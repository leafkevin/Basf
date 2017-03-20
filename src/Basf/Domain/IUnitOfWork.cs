using System;

namespace Basf.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        void Begin();
        void Commit();
        void Rollback();
    }
}
