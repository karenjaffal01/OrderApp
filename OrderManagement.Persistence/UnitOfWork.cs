using OrderManagement.Domain.Entities;
using OrderManagement.Persistence.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace OrderManagement.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;

        public IItemRepository Items { get; }
        public IStockRepository Stock { get; }
        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }
        public IDbConnection Connection => _connection;
        public IDbTransaction Transaction => _transaction!;

        public UnitOfWork(IDbConnection connection,
                          IItemRepository items,
                          IStockRepository stock,
                          IOrderRepository orders,
                          IOrderItemRepository orderItems)
        {
            _connection = connection;
            Items = items;
            Stock = stock;
            Orders = orders;
            OrderItems = orderItems;
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
