namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class SagaWithDependencyContext : SagaDbContext<SagaWithDependency, SagaWithDependencyMap>
    {
        public SagaWithDependencyContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SagaDependency>().HasOne(it => it.SagaInnerDependency).WithMany().IsRequired();

            ConfigureShadowId(modelBuilder.Entity<SagaDependency>());
            ConfigureShadowId(modelBuilder.Entity<SagaInnerDependency>());
        }

        private static void ConfigureShadowId<T>(
            EntityTypeBuilder<T> entity, string idPropertyName = "Id") where T : class
        {
            entity.Property<Guid>(idPropertyName).IsRequired().ValueGeneratedOnAdd();
            entity.HasKey(idPropertyName);
        }
    }
}