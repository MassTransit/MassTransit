namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internals.Extensions;
    using Mappings;
    using MassTransit.Saga;
    using Metadata;
    using Microsoft.EntityFrameworkCore;


    public abstract class SagaDbContext :
        DbContext
    {
        protected abstract IEnumerable<ISagaClassMap> Configurations { get; }

        protected SagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var configuration in Configurations)
                configuration.Configure(modelBuilder);
        }

        public override DbSet<TEntity> Set<TEntity>()
        {
        #if DEBUG
            if (typeof(TEntity).HasInterface<ISaga>() && !ContainsSagaConfiguration<TEntity>())
                throw new ApplicationException($"Context should contains {TypeMetadataCache<TEntity>.ShortName} configuration");
        #endif
            return base.Set<TEntity>();
        }

        bool ContainsSagaConfiguration<T>()
        {
            return Configurations != null && Configurations.Any(x => x.SagaType == typeof(T));
        }
    }
}
