using System.Data.Entity.ModelConfiguration;
using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
    public class EntityBaseConfiguration<TEntity> : EntityTypeConfiguration<TEntity>
        where TEntity : class, IEntityBase
    {
        public EntityBaseConfiguration()
        {
            HasKey(e => e.Id);
        }
    }
}
