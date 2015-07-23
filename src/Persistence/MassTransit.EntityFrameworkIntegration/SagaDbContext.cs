namespace MassTransit.EntityFrameworkIntegration
{
    using System.Data.Entity;
    using System.Data.Common;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using MassTransit.Saga;

    public class SagaDbContext<TSaga, TSagaClassMapping> : DbContext
        where TSaga : SagaEntity, ISaga
        where TSagaClassMapping : SagaClassMapping<TSaga>, new()
    {
        #region Constructors
        protected SagaDbContext() { }

        protected SagaDbContext(DbCompiledModel model)
            : base(model)
        { }

        public SagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }

        public SagaDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        { }

        public SagaDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        { }

        public SagaDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        { }

        public SagaDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        { }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TSagaClassMapping());
        }
    }
}
