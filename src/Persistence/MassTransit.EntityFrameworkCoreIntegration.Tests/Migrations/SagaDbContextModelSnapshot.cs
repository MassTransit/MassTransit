using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MassTransit.EntityFrameworkCoreIntegration;

using MassTransit.Tests.Saga;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations
{
    [DbContext(typeof(SagaDbContext<SimpleSaga, SimpleSagaMap>))]
    partial class SagaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.Tests.Saga.SimpleSaga", b =>
                {
                    b.Property<Guid>("CorrelationId");

                    b.Property<bool>("Completed");

                    b.Property<bool>("Initiated");

                    b.Property<string>("Name")
                        .HasMaxLength(40);

                    b.Property<bool>("Observed");

                    b.HasKey("CorrelationId");

                    b.ToTable("EfCoreSimpleSagas");
                });
        }
    }
}
