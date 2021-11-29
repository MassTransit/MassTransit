namespace MassTransit.EntityFrameworkCoreIntegration.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


    class EntityFrameworkSagaRepository :
        IEntityFrameworkSagaRepository
    {
        readonly ConcurrentDictionary<Type, ISagaClassMap> _configurations;
        readonly DbContextOptions _dbContextOptions;

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

        public DbContext GetDbContext()
        {
            return new RepositorySagaDbContext(_dbContextOptions, _configurations.Values);
        }

        public static DbContextOptionsBuilder CreateOptionsBuilder()
        {
            return new DbContextOptionsBuilder<RepositorySagaDbContext>();
        }


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
