namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ISagaClassMap
    {
        Type SagaType { get; }
        void Configure(ModelBuilder model);
    }


    public interface ISagaClassMap<TSaga> :
        ISagaClassMap
        where TSaga : class, ISaga
    {
    }
}
