using Poc.Web.Entities;
using System;
using System.Linq;

namespace Poc.Web.Repository
{
    public interface IRepository<T> where T : EntidadeBase
    {
        void Salvar();
        void Inserir(T entidade);
        void Remover(T entidade);
        T RetornarPorId(Guid id);
        T RetornarPrimeiraOcorrencia();
        IQueryable<T> Consulta();
    }
}
