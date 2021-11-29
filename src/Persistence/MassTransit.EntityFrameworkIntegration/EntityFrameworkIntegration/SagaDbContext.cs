namespace MassTransit.EntityFrameworkIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Internals;


    public abstract class SagaDbContext :
        DbContext
    {
        protected SagaDbContext()
        {
        }

        protected SagaDbContext(DbCompiledModel model)
            : base(model)
        {
        }

        protected SagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected SagaDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected SagaDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        protected SagaDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        protected SagaDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        protected abstract IEnumerable<ISagaClassMap> Configurations { get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var configuration in Configurations)
                configuration.Configure(modelBuilder);
        }

        public override DbSet<TEntity> Set<TEntity>()
        {
        #if DEBUG
            if (typeof(TEntity).HasInterface<ISaga>() && !ContainsSagaConfiguration<TEntity>())
                throw new ApplicationException($"Context should contains {TypeCache<TEntity>.ShortName} configuration");
        #endif
            return base.Set<TEntity>();
        }

        bool ContainsSagaConfiguration<T>()
        {
            return Configurations != null && Configurations.Any(x => x.SagaType == typeof(T));
        }
    }
}
