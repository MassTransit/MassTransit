namespace MassTransit.NinjectIntegration
{
    using System;
    using System.Collections.Generic;
    using Saga;


    public interface ISagaRegistry
    {
        ICachedConfigurator Add<T>()
            where T : class, ISaga;

        ICachedConfigurator Add(Type sagaType);

        IEnumerable<ICachedConfigurator> GetConfigurators();
    }
}
