using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using MassTransit.Tests.Saga;

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Migrations.SlowConcurrentSaga
{
    using SimpleSaga.DataAccess;
    using Tests.SlowConcurrentSaga.DataAccess;


    [DbContext(typeof(SlowConcurrentSagaDbContext))]
    [Migration("20170710150441_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga.DataAccess.SlowConcurrentSaga", b =>
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
