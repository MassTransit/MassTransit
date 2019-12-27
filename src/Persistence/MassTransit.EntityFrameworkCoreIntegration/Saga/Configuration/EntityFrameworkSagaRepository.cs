namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Configuration
{
    using System;
    using System.Collections.Generic;
    using Mappings;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    class EntityFrameworkSagaRepository : IEntityFrameworkSagaRepository
    {
        readonly DbContextOptions _dbContextOptions;
        readonly IList<ISagaClassMap> _configurations;

        public EntityFrameworkSagaRepository(DbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            _configurations = new List<ISagaClassMap>();
        }

        public void ConfigureSaga<TSaga>(Action<EntityTypeBuilder<TSaga>> configure = null)
            where TSaga : class, ISaga
        {
            _configurations.Add(new ActionSagaClassMap<TSaga>(configure));
        }

        public SagaDbContext GetDbContext() => new RepositorySagaDbContext(_dbContextOptions, _configurations);

        public static DbContextOptionsBuilder CreateOptionsBuilder() => new DbContextOptionsBuilder<RepositorySagaDbContext>();


        class RepositorySagaDbContext : SagaDbContext
        {
            public RepositorySagaDbContext(DbContextOptions options, IEnumerable<ISagaClassMap> configurations)
                : base(options)
            {
                Configurations = configurations;
            }

            protected override IEnumerable<ISagaClassMap> Configurations { get; }
        }


        class ActionSagaClassMap<T> : SagaClassMap<T>
            where T : class, ISaga
        {
            readonly Action<EntityTypeBuilder<T>> _configure;

            public ActionSagaClassMap(Action<EntityTypeBuilder<T>> configure)
            {
                _configure = configure;
            }

            protected override void Configure(EntityTypeBuilder<T> cfg, ModelBuilder modelBuilder)
            {
                base.Configure(cfg, modelBuilder);
                _configure?.Invoke(cfg);
            }
        }
    }
}
