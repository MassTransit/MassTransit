namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class FutureStateMap :
        SagaClassMap<FutureState>
    {
        readonly bool _optimistic;

        public FutureStateMap(bool optimistic)
        {
            _optimistic = optimistic;
        }

        protected override void Configure(EntityTypeBuilder<FutureState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);

            if (_optimistic)
            {
                entity.Property(x => x.RowVersion)
                    .IsRowVersion();
            }
            else
                entity.Ignore(x => x.RowVersion);

            entity.Property(x => x.Created);
            entity.Property(x => x.Completed);
            entity.Property(x => x.Faulted);

            entity.Property(x => x.Location);

            entity.Property(x => x.Command)
                .HasJsonConversion();
            entity.Property(x => x.Pending)
                .HasJsonConversion();
            entity.Property(x => x.Subscriptions)
                .HasJsonConversion();
            entity.Property(x => x.Variables)
                .HasJsonConversion();
            entity.Property(x => x.Results)
                .HasJsonConversion();
            entity.Property(x => x.Faults)
                .HasJsonConversion();
        }
    }
}
