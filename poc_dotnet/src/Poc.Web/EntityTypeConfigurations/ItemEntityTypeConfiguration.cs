using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poc.Web.Entities;
using Poc.Web.Extensions;

namespace Poc.Web.EntityTypeConfigurations
{
  public class ItemEntityTypeConfiguration : EntityTypeConfiguration<Item>
  {
    public override void Map(EntityTypeBuilder<Item> builder)
    {
      builder.ToTable("Item");
      builder.HasKey(c => c.Id);
      builder.Property(c => c.Id);
      builder.Property(c => c.Descricao).HasMaxLength(50).IsRequired();
      builder.Property(c => c.Preco).HasColumnType("decimal(5,2)");
      builder.Property(c => c.DataCriacao);
      builder.HasOne(a => a.Pedido).WithMany(a => a.Itens).HasForeignKey(a => a.PedidoId);
    }
  }
}
