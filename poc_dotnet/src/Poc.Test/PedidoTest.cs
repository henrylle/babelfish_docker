using NUnit.Framework;
using Poc.Web.Controllers;
using Poc.Web.DTO;
using Poc.Web.Repository;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Poc.Test
{
  public class PedidoTest : BaseTest
  {
    private PedidosController _sut;

    [SetUp]
    public void ConfiguracaoAntesDeCadaTeste()
        => _sut = _container.GetInstance<PedidosController>();

    [Test]
    public void RetonaOkAoInserirPedidoTest()
    {
      //Ambiente
      #region Pedido
      var pedido = new PedidoDTO()
      {
        Nome = "Pedido1",
        Itens = new List<ItemDTO>()
                {
                    new ItemDTO
                    {
                        Descricao = "Item1",
                        Preco = 100
                    },
                    new ItemDTO
                    {
                        Descricao = "Item2",
                        Preco = 200
                    },
                }
      };
      #endregion

      //Acao
      var response = _sut.Inserir(pedido).Result;

      var result = ((Microsoft.AspNetCore.Mvc.ObjectResult)response);

      //Assertivas
      Assert.AreEqual(200, result.StatusCode);
      Assert.AreEqual("Pedido inserido com sucesso", result.Value);

      var pedidoPersistido = _container.GetInstance<IRepository<Web.Entities.Pedido>>().Consulta().Include(a => a.Itens).FirstOrDefault();
      Assert.AreEqual(pedido.Nome, pedidoPersistido.Nome);
      Assert.AreEqual(pedido.Itens.Count, pedidoPersistido.Itens.Count);
    }

    [Test]
    public void RetonaOkAoExcluirPedidoTest()
    {
      //Ambiente
      RetonaOkAoInserirPedidoTest();
      var pedido = _container.GetInstance<IRepository<Web.Entities.Pedido>>().RetornarPrimeiraOcorrencia();

      //Acao
      var response = _sut.Excluir(pedido.Id.ToString()).Result;

      var result = ((Microsoft.AspNetCore.Mvc.ObjectResult)response);

      //Assertivas
      Assert.AreEqual(200, result.StatusCode);
      Assert.AreEqual("Pedido removido com sucesso", result.Value);

      var pedidoPersistido = _container.GetInstance<IRepository<Web.Entities.Pedido>>().RetornarPorId(pedido.Id);
      Assert.IsNull(pedidoPersistido);
    }
  }
}