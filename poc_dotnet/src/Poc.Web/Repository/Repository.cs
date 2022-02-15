using Microsoft.EntityFrameworkCore;
using Poc.Web.Entities;
using System;
using System.Linq;

namespace Poc.Web.Repository
{
    public class Repository<T> : IRepository<T> where T : EntidadeBase
    {
        protected IUnitOfWork Context;

        public Repository(IUnitOfWork unitOfWork) => Context = unitOfWork;

        public virtual void Inserir(T entidade)
        {
            try
            {
                Context.Set<T>().Add(entidade);
                Salvar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Remover(T entidade)
        {
            Context.Set<T>().Remove(entidade);
            Salvar();
        }

        public virtual void Atualizar(T entidade)
        {
            var entityEntry = Context.Entry(entidade);
            entityEntry.State = EntityState.Modified;
            Salvar();
        }

        public T RetornarPrimeiraOcorrencia()
            => Context.Set<T>().FirstOrDefault();

        public T RetornarPorId(Guid id)
            => Context.Set<T>().FirstOrDefault(a=>a.Id == id);

        public IQueryable<T> Consulta()
            => Context.Set<T>();

        public void Salvar() => Context.SaveChanges();
    }
}
