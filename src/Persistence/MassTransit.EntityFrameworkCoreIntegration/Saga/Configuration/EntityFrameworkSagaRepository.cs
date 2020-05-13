namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Mappings;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    class EntityFrameworkSagaRepository :
        IEntityFrameworkSagaRepository
    {
        readonly DbContextOptions _dbContextOptions;
        readonly ConcurrentDictionary<Type, ISagaClassMap> _configurations;

        public EntityFrameworkSagaRepository(DbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            _configurations = new ConcurrentDictionary<Type, ISagaClassMap>();
        }

        public void AddSagaClassMap<TSaga>(ISagaClassMap<TSaga> sagaClassMap)
            where TSaga : class, ISaga
        {
            if (sagaClassMap == null)
                throw new ArgumentNullException(nameof(sagaClassMap));
            _configurations.GetOrAdd(sagaClassMap.SagaType, sagaClassMap);
        }

        public DbContext GetDbContext() => new RepositorySagaDbContext(_dbContextOptions, _configurations.Values);

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
    }
}
