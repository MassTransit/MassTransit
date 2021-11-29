namespace MassTransit.DependencyInjection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        public static readonly IStateMachineActivityFactory Instance = new DependencyInjectionStateMachineActivityFactory();

        public T GetService<T>(PipeContext context)
            where T : class
        {
            if (context.TryGetPayload(out IServiceScope serviceScope))
                return ActivatorUtilities.GetServiceOrCreateInstance<T>(serviceScope.ServiceProvider);

            if (context.TryGetPayload(out IServiceProvider serviceProvider))
                return ActivatorUtilities.GetServiceOrCreateInstance<T>(serviceProvider);

            throw new PayloadNotFoundException($"IServiceProvider or IServiceScope was not found to get service: {TypeCache<T>.ShortName}");
        }
    }
}
