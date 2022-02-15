using MediatR;
using Poc.Web.Extensions;

namespace Poc.Web.Command
{
    public class PedidoExcluirCommand : IRequest<OperationResult<string>>
    {
        public PedidoExcluirCommand(string id) => Id = id;
        public string Id { get; set; }
    }
}
