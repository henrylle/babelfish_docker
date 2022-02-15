using MediatR;
using Poc.Web.Entities;
using Poc.Web.Extensions;
using Poc.Web.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.Web.Command
{
    public class PedidoCommandHandler : HandlerBase,
        IRequestHandler<PedidoInserirCommand, OperationResult<string>>,
        IRequestHandler<PedidoExcluirCommand, OperationResult<string>>
    {
        private readonly IRepository<Pedido> _pedidoRepository;

        public PedidoCommandHandler(IRepository<Pedido> pedidoRepository)
            => _pedidoRepository = pedidoRepository;

        Task<OperationResult<string>> IRequestHandler<PedidoInserirCommand, OperationResult<string>>.Handle(PedidoInserirCommand request, CancellationToken cancellationToken)
          => CallFunction(() => IncluirPedido(request));

        public Task<OperationResult<string>> Handle(PedidoExcluirCommand request, CancellationToken cancellationToken)
          => CallFunction(() => ExcluirPedido(request.Id));

        private string IncluirPedido(PedidoInserirCommand request)
        {
            var pedido = new Pedido(request.Pedido.Nome);

            foreach (var item in request.Pedido.Itens)
            {
                pedido.Itens.Add(new Item(item.Descricao, item.Preco));
            }

            _pedidoRepository.Inserir(pedido);

            return "Pedido inserido com sucesso";
        }
        private string ExcluirPedido(string id)
        {
            var pedido = _pedidoRepository.RetornarPorId(id.ToGuid());
            _pedidoRepository.Remover(pedido);

            return "Pedido removido com sucesso";
        }
    }
}
