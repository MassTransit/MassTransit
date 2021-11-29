namespace MassTransit.EntityFrameworkIntegration
{
    using System;
    using System.Data.Entity;


    public interface ISagaClassMap
    {
        Type SagaType { get; }
        void Configure(DbModelBuilder modelBuilder);
    }
}
