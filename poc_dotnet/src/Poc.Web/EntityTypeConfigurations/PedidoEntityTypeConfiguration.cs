using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poc.Web.Entities;
using Poc.Web.Extensions;

namespace Poc.Web.EntityTypeConfigurations
{
    public class PedidoEntityTypeConfiguration : EntityTypeConfiguration<Pedido>
    {
        public override void Map(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedido");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id);
            builder.Property(c => c.Nome).HasMaxLength(50).IsRequired();
            builder.Property(c => c.DataCriacao);
            builder.HasMany(a => a.Itens).WithOne().HasForeignKey(a => a.PedidoId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
