using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using MassTransit.Tests.Saga;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.DeadlockSaga
{
    using MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.DataAccess;

    [DbContext(typeof(DeadlockSagaDbContext))]
    [Migration("20170710150441_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga.DataAccess.DeadlockSaga", b =>
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
