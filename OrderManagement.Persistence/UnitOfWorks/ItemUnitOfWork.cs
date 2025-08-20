using OrderManagement.Persistence.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace OrderManagement.Persistence.UnitOfWorks
{

    public class ItemUnitOfWork : IItemUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;

        public IItemRepository Items { get; }
        public IDbConnection Connection => _connection;
        public IDbTransaction Transaction => _transaction!;

        public ItemUnitOfWork(IDbConnection connection, IItemRepository items)
        {
            _connection = connection;
            Items = items;
        }

        public async Task BeginTransactionAsync()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            _transaction = _connection.BeginTransaction();
            await Task.CompletedTask;
        }

        public async Task CommitAsync()
        {
            _transaction?.Commit();
            await Task.CompletedTask;
        }

        public async Task RollbackAsync()
        {
            _transaction?.Rollback();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
