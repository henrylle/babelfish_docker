using System;

namespace Poc.Web.Entities
{
    public abstract class EntidadeBase
    {
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        public static implicit operator bool(EntidadeBase entidade) => entidade != null;

        public override int GetHashCode() => Id.GetHashCode();

        protected bool Equals(EntidadeBase other) => Equals(Id, other.Id);

        public override bool Equals(object obj)
        {
            if (string.IsNullOrEmpty(obj.ToString()))
                return false;

            var objComparacao = (EntidadeBase)obj;
            return Id == objComparacao.Id;
        }

        public DateTimeOffset DataCriacao { get; set; } = DateTimeOffset.UtcNow;
    }
}
