namespace MassTransit.NinjectIntegration
{
    using System;
    using System.Collections.Generic;


    public interface IConsumerRegistry
    {
        ICachedConfigurator Add<T>()
            where T : class, IConsumer;

        ICachedConfigurator Add(Type consumerType);

        IEnumerable<ICachedConfigurator> GetConfigurators();
    }
}
