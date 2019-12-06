namespace MassTransit.Registration
{
    using System;
    using Saga;


    public interface ISagaRepositoryFactory
    {
        ISagaRepository<T> CreateSagaRepository<T>(Action<ConsumeContext> scopeAction = null)
            where T : class, ISaga;
    }
}
