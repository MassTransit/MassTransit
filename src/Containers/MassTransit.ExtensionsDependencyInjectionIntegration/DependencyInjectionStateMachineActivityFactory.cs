namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Automatonymous;
    using GreenPipes;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Util;


    public class DependencyInjectionStateMachineActivityFactory :
        IStateMachineActivityFactory
    {
        Activity<TInstance, TData> IStateMachineActivityFactory.GetActivity<TActivity, TInstance, TData>(BehaviorContext<TInstance, TData> context)
        {
            return GetActivity<TActivity>(context);
        }

        Activity<TInstance> IStateMachineActivityFactory.GetActivity<TActivity, TInstance>(BehaviorContext<TInstance> context)
        {
            return GetActivity<TActivity>(context);
        }

        static TActivity GetActivity<TActivity>(PipeContext context)
        {
            if (context.TryGetPayload(out IServiceScope serviceScope))
                return serviceScope.ServiceProvider.GetRequiredService<TActivity>();

            if (context.TryGetPayload(out IServiceProvider serviceProvider))
                return serviceProvider.GetRequiredService<TActivity>();

            throw new PayloadNotFoundException($"IServiceProvider or IServiceScope was not found to create activity: {TypeMetadataCache<TActivity>.ShortName}");
        }
    }
}
