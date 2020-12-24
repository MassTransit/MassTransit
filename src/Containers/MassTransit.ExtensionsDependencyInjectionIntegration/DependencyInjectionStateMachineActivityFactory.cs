namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Automatonymous;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new DependencyInjectionStateMachineActivityFactory();

        public T GetService<T>(PipeContext context)
            where T : class
        {
            if (context.TryGetPayload(out IServiceScope serviceScope))
                return serviceScope.ServiceProvider.GetService<T>() ?? ActivatorUtilities.CreateInstance<T>(serviceScope.ServiceProvider);

            if (context.TryGetPayload(out IServiceProvider serviceProvider))
                return serviceProvider.GetService<T>() ?? ActivatorUtilities.CreateInstance<T>(serviceProvider);

            throw new PayloadNotFoundException($"IServiceProvider or IServiceScope was not found to get service: {TypeCache<T>.ShortName}");
        }
    }
}
