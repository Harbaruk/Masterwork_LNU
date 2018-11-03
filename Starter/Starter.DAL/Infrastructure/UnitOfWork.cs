using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;

namespace Starter.DAL.Infrastructure
{
    public sealed class EFUnitOfWork : IUnitOfWork
    {
        private readonly ProjectDbContext _dbContext;
        private bool _disposed;
        private Dictionary<string, object> _repositories;

        public EFUnitOfWork(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<T> Repository<T>()
            where T : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(EFRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);
                _repositories.Add(type, repositoryInstance);
            }
            return (EFRepository<T>)_repositories[type];
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public IDbTransaction BeginTransaction()
        {
            IDbContextTransaction transaction = _dbContext.Database.BeginTransaction();
            return new EfDbTransaction(transaction);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }
    }

    internal sealed class EfDbTransaction : IDbTransaction
    {
        private IDbContextTransaction _transaction;

        internal EfDbTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit() => _transaction.Commit();

        public void Rollback() => _transaction.Rollback();

        public void Dispose() => _transaction.Dispose();
    }
}