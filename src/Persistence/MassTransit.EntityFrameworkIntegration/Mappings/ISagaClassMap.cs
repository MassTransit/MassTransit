namespace MassTransit.EntityFrameworkIntegration.Mappings
{
    using System;
    using System.Data.Entity;


    public interface ISagaClassMap
    {
        Type SagaType { get; }
        void Configure(DbModelBuilder modelBuilder);
    }
}
