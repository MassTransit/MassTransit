namespace MassTransit.EntityFrameworkCoreIntegration.Mappings
{
    using System;
    using Microsoft.EntityFrameworkCore;


    public interface ISagaClassMap
    {
        Type SagaType { get; }
        void Configure(ModelBuilder modelBuilder);
    }
}
