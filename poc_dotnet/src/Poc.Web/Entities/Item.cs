using System;

namespace Poc.Web.Entities
{
    public class Item : EntidadeBase
    {
        public Item(string descricao, decimal preco)
        {
            Descricao = descricao;
            Preco = preco;
        }

        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public Guid PedidoId { get; set; }
        public Pedido Pedido { get; set; }
    }
}
