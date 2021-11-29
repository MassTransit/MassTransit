namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency.DataAccess
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class SagaWithDependencyMap :
        SagaClassMap<SagaWithDependency>
    {
        protected override void Configure(EntityTypeBuilder<SagaWithDependency> entity, ModelBuilder model)
        {
            entity.Property(x => x.Name).HasMaxLength(40);
            entity.Property(x => x.Initiated);
            entity.Property(x => x.Completed);
            entity.ToTable("EfCoreSagasWithDepencies");

            entity.HasOne(x => x.Dependency).WithMany().IsRequired();

            ConfigureSagaDependency(model);
            ConfigureSagaInnerDependency(model);
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
