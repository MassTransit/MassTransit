// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.LamarIntegration.Registration
{
    using System;
    using Lamar;
    using MassTransit.Registration;
    using ScopeProviders;
    using Scoping;


    public class ServiceRegistryRegistrationConfigurator :
        RegistrationConfigurator,
        IServiceRegistryConfigurator
    {
        readonly ServiceRegistry _registry;

        public ServiceRegistryRegistrationConfigurator(ServiceRegistry registry)
            : base(new LamarContainerRegistrar(registry))
        {
            _registry = registry;

            registry.For<IConsumerScopeProvider>()
                .Use(CreateConsumerScopeProvider)
                .Singleton();

            registry.For<ISagaRepositoryFactory>()
                .Use(CreateSagaRepositoryFactory)
                .Singleton();

            registry.For<IConfigurationServiceProvider>()
                .Use(context => new LamarConfigurationServiceProvider(context.GetInstance<IContainer>()))
                .Singleton();

            registry.For<IRegistrationConfigurator>()
                .Use(this);

            registry.For<IRegistration>()
                .Use(provider => CreateRegistration(provider.GetInstance<IConfigurationServiceProvider>()))
                .Singleton();

            registry.Injectable<ConsumeContext>();
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IServiceContext context)
        {
            return new LamarConsumerScopeProvider(context.GetInstance<IContainer>());
        }

        ISagaRepositoryFactory CreateSagaRepositoryFactory(IServiceContext context)
        {
            return new LamarSagaRepositoryFactory(context.GetInstance<IContainer>());
        }

        ServiceRegistry IServiceRegistryConfigurator.Builder => _registry;

        public void AddBus(Func<IServiceContext, IBusControl> busFactory)
        {
            _registry.For<IBusControl>()
                .Use(busFactory)
                .Singleton();

            _registry.For<IBus>()
                .Use(context => context.GetInstance<IBusControl>())
                .Singleton();

            _registry.For<ISendEndpointProvider>()
                .Use(GetCurrentSendEndpointProvider)
                .Scoped();

            _registry.For<IPublishEndpoint>()
                .Use(GetCurrentPublishEndpoint)
                .Scoped();

            _registry.For<IClientFactory>()
                .Use(context => context.GetInstance<IBus>().CreateClientFactory())
                .Singleton();
        }

        public void AddRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _registry.For<IRequestClient<T>>().Use(context =>
            {
                var clientFactory = context.GetInstance<IClientFactory>();

                ConsumeContext consumeContext = context.TryGetInstance<ConsumeContext>();
                return (consumeContext != null)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            }).Scoped();
        }

        public void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _registry.For<IRequestClient<T>>().Use(context =>
            {
                var clientFactory = context.GetInstance<IClientFactory>();

                ConsumeContext consumeContext = context.TryGetInstance<ConsumeContext>();
                return (consumeContext != null)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            }).Scoped();
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IServiceContext context)
        {
            return context.TryGetInstance<ConsumeContext>() ?? (ISendEndpointProvider)context.GetInstance<IBus>();
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IServiceContext context)
        {
            return context.TryGetInstance<ConsumeContext>() ?? (IPublishEndpoint)context.GetInstance<IBus>();
        }
    }
}
