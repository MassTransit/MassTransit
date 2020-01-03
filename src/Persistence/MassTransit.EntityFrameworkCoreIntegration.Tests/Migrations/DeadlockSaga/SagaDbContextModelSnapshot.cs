namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.DeadlockSaga
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Tests.DeadlockSaga.DataAccess;


    [DbContext(typeof(DeadlockSagaDbContext))]
    partial class SlowConcurrentSagaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.DeadlockSaga", b =>
                {
                    b.Property<Guid>("CorrelationId");

                    b.Property<string>("CurrentState")
                        .HasMaxLength(40);

                    b.Property<string>("Name")
                        .HasMaxLength(40);

                    b.Property<int>("Counter");

                    b.HasKey("CorrelationId");

                    b.ToTable("EfCoreSlowConcurrentSagas");
                });
        }
    }
}
