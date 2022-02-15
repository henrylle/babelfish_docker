using MediatR;
using Poc.Web.DTO;
using Poc.Web.Extensions;

namespace Poc.Web.Command
{
    public class PedidoInserirCommand : IRequest<OperationResult<string>>
    {
        public PedidoInserirCommand(PedidoDTO pedido) => Pedido = pedido;
        public PedidoDTO Pedido { get; set; }
    }
}
