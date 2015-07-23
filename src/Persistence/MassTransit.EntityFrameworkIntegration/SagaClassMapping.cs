namespace MassTransit.EntityFrameworkIntegration
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public abstract class SagaClassMapping<T> :
        EntityTypeConfiguration<T>
        where T : SagaEntity
    {
        protected SagaClassMapping()
        {
            HasKey(t => t.CorrelationId);

            Property(t => t.CorrelationId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}
