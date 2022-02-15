using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Poc.Web.Entities;
using Poc.Web.EntityTypeConfigurations;
using Poc.Web.Extensions;
using System;
using System.Data;
using System.IO;

namespace Poc.Web.Repository
{
    public class PocContext : DbContext, IUnitOfWork
    {
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Item> Item { get; set; }

        public static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json")
                       .Build();

            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(LoggerFactory)
                .UseSqlServer(config.GetConnectionString(Environment.MachineName) ?? config.GetConnectionString("PocAuroraConnectionString"), sqlServerOptions => sqlServerOptions.CommandTimeout(20));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddConfiguration(new PedidoEntityTypeConfiguration());
            modelBuilder.AddConfiguration(new ItemEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        #region IUoW

       
        private IDbContextTransaction _dbContextTransaction;
        private bool _rolledback;

        public void Commit() => _dbContextTransaction.Commit();

        public void Rollback()
        {
            if (Database.CurrentTransaction == null || _rolledback) return;
            Database.CurrentTransaction.Rollback();
            _rolledback = true;
        }

        public IDbContextTransaction StartTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
            => _dbContextTransaction = Database.BeginTransaction(isolationLevel);

        public void Save()
        {
            try
            {
                ChangeTracker.DetectChanges();
                SaveChanges();
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        public void SaveAndCommit()
        {
            try
            {
                Save();
                Commit();
            }
            catch
            {
                //Dispose não deve lançar exceção
            }
        }

        public override void Dispose()
        {
            if (_rolledback)
                Rollback();
            else if (Database.CurrentTransaction != null && !_rolledback)
                SaveAndCommit();
            base.Dispose();
        }

        #endregion
    }
}
