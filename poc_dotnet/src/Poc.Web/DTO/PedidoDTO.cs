using System.Collections.Generic;

namespace Poc.Web.DTO
{
    public class PedidoDTO
    {
        public string Nome { get; set; }
        public IList<ItemDTO> Itens { get; set; }
    }
}
