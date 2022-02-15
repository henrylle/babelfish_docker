using System.Collections.Generic;

namespace Poc.Web.Entities
{
    public class Pedido : EntidadeBase
    {
        public string Nome { get; set; }

        public Pedido(string nome)
        {
            Nome = nome;
            Itens = new List<Item>();
        }

        public IList<Item> Itens { get; set; }
    }
}
