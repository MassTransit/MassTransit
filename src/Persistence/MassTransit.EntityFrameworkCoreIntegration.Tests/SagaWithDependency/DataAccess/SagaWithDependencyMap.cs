namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency.DataAccess
{
    using System;
    using Mappings;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class SagaWithDependencyMap :
        SagaClassMap<SagaWithDependency>
    {
        protected override void Configure(EntityTypeBuilder<SagaWithDependency> entityTypeBuilder, ModelBuilder modelBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.Initiated);
            entityTypeBuilder.Property(x => x.Completed);
            entityTypeBuilder.ToTable("EfCoreSagasWithDepencies");

            entityTypeBuilder.HasOne(x => x.Dependency).WithMany().IsRequired();

            ConfigureSagaDependency(modelBuilder);
            ConfigureSagaInnerDependency(modelBuilder);
        }

        static void ConfigureSagaDependency(ModelBuilder modelBuilder, string id = "Id")
        {
            EntityTypeBuilder<SagaDependency> builder = modelBuilder.Entity<SagaDependency>();
            builder.Property<Guid>(id).IsRequired().ValueGeneratedOnAdd();
            builder.HasKey(id);
            builder.HasOne(it => it.SagaInnerDependency).WithMany().IsRequired();
        }

        static void ConfigureSagaInnerDependency(ModelBuilder modelBuilder, string id = "Id")
        {
            EntityTypeBuilder<SagaInnerDependency> builder = modelBuilder.Entity<SagaInnerDependency>();
            builder.Property<Guid>(id).IsRequired().ValueGeneratedOnAdd();
            builder.HasKey(id);
        }
    }
}
