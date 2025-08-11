using OrderManagement.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private IDbTransaction? _transaction;

        public IOrderRepository Orders { get; }
        public IOrderItemRepository OrderItems { get; }

        public IDbConnection Connection => _connection;
        public IDbTransaction Transaction => _transaction!;

        public UnitOfWork(IDbConnection connection,
                          IOrderRepository orderRepository,
                          IOrderItemRepository orderItemRepository)
        {
            _connection = connection;
            Orders = orderRepository;
            OrderItems = orderItemRepository;
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
