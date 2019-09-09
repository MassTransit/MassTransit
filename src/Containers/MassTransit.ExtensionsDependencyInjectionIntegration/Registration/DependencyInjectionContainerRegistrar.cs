namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using Automatonymous;
    using Courier;
    using Definition;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class DependencyInjectionContainerRegistrar :
        IContainerRegistrar
    {
        readonly IServiceCollection _collection;

        public DependencyInjectionContainerRegistrar(IServiceCollection collection)
        {
            _collection = collection;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _collection.AddScoped<T>();
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _collection.AddTransient<IConsumerDefinition<TConsumer>, TDefinition>();
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _collection.TryAddSingleton<IStateMachineActivityFactory, DependencyInjectionStateMachineActivityFactory>();
            _collection.TryAddSingleton<ISagaStateMachineFactory, DependencyInjectionSagaStateMachineFactory>();

            _collection.AddSingleton<TStateMachine>();
            _collection.AddSingleton<SagaStateMachine<TInstance>>(provider => provider.GetRequiredService<TStateMachine>());
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _collection.AddTransient<ISagaDefinition<TSaga>, TDefinition>();
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _collection.TryAddScoped<TActivity>();

            _collection.AddTransient<IExecuteActivityScopeProvider<TActivity, TArguments>,
                DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>>();
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _collection.AddTransient<IActivityDefinition<TActivity, TArguments, TLog>, TDefinition>();
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _collection.AddTransient<IExecuteActivityDefinition<TActivity, TArguments>, TDefinition>();
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _collection.AddTransient<IEndpointDefinition<T>, TDefinition>();

            if (settings != null)
                _collection.AddSingleton(settings);
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _collection.AddSingleton(provider =>
            {
                var clientFactory = provider.GetRequiredService<IClientFactory>();

                return clientFactory.CreateRequestClient<T>(timeout);
            });

            _collection.AddScoped(context =>
            {
                var clientFactory = context.GetRequiredService<IClientFactory>();

                var consumeContext = context.GetService<ConsumeContext>();
                return consumeContext != null
                    ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            });
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _collection.AddSingleton(provider =>
            {
                var clientFactory = provider.GetRequiredService<IClientFactory>();

                return clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            });

            _collection.AddScoped(context =>
            {
                var clientFactory = context.GetRequiredService<IClientFactory>();

                var consumeContext = context.GetService<ConsumeContext>();
                return consumeContext != null
                    ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            });
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            _collection.TryAddScoped<TActivity>();

            _collection.AddTransient<ICompensateActivityScopeProvider<TActivity, TLog>, DependencyInjectionCompensateActivityScopeProvider<TActivity, TLog>>();
        }
    }
}
