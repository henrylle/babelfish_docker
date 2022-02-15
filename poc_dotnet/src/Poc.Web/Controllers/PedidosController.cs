using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poc.Web.Command;
using Poc.Web.DTO;
using System.Threading.Tasks;

namespace Poc.Web.Controllers
{
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidosController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Inserir(PedidoDTO pedido)
        {
            var resp = await _mediator.Send(new PedidoInserirCommand(pedido));

            return resp.IsSuccess
                ? Ok(resp.Result) 
                : BadRequest(resp.Exception.Message);
        }


        [HttpDelete]
        public async Task<IActionResult> Excluir(string id)
        {
            var resp = await _mediator.Send(new PedidoExcluirCommand(id));

            return resp.IsSuccess
                ? Ok(resp.Result)
                : BadRequest(resp.Exception.Message);
        }
    }
}
