namespace MassTransit.EntityFrameworkCoreIntegration.Mappings
{
    using System;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public interface ISagaClassMap
    {
        Type SagaType { get; }
        void Configure(ModelBuilder modelBuilder);
    }


    public interface ISagaClassMap<TSaga> :
        ISagaClassMap
        where TSaga : class, ISaga
    {
    }
}
