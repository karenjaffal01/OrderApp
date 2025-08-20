using System;
using System.Data;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Interfaces
{
    public interface ILoginUnitOfWork : IDisposable
    {
        ILoginRepository Login { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
