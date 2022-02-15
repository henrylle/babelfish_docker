using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Poc.Web.Entities;
using System;
using System.Data;

namespace Poc.Web.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        #region IoW
        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        DbSet<T> Set<T>() where T : class;
        EntityEntry Entry(object targetValue);
        int SaveChanges();

        void Commit();
        void Rollback();

        IDbContextTransaction StartTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        #endregion

        DbSet<Pedido> Pedido { get; set; }
        DbSet<Item> Item { get; set; }
    }
}
