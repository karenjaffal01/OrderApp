using System;
using System.Data;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.Interfaces
{
    public interface IStockUnitOfWork : IDisposable
    {
        IStockRepository Stock { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}

