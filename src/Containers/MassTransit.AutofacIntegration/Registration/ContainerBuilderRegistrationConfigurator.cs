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
namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using ScopeProviders;
    using Scoping;


    public class ContainerBuilderRegistrationConfigurator :
        RegistrationConfigurator,
        IContainerBuilderConfigurator
    {
        readonly ContainerBuilder _builder;
        Action<ContainerBuilder, ConsumeContext> _configureScope;

        public ContainerBuilderRegistrationConfigurator(ContainerBuilder builder)
            : base(new AutofacContainerRegistrar(builder))
        {
            _builder = builder;

            ScopeName = "message";

            builder.Register(CreateConsumerScopeProvider)
                .As<IConsumerScopeProvider>()
                .SingleInstance();

            builder.Register(CreateSagaRepositoryFactory)
                .As<ISagaRepositoryFactory>()
                .SingleInstance();

            builder.Register(context => new AutofacConfigurationServiceProvider(context.Resolve<ILifetimeScope>()))
                .As<IConfigurationServiceProvider>()
                .SingleInstance();

            builder.RegisterInstance<IRegistrationConfigurator>(this);

            builder.Register(provider => CreateRegistration(provider.Resolve<IConfigurationServiceProvider>()))
                .As<IRegistration>()
                .SingleInstance();
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacConsumerScopeProvider(lifetimeScopeProvider, ScopeName, _configureScope);
        }

        ISagaRepositoryFactory CreateSagaRepositoryFactory(IComponentContext context)
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacSagaRepositoryFactory(lifetimeScopeProvider, ScopeName, _configureScope);
        }

        public string ScopeName { private get; set; }

        ContainerBuilder IContainerBuilderConfigurator.Builder => _builder;

        public Action<ContainerBuilder, ConsumeContext> ConfigureScope
        {
            set => _configureScope = value;
        }

        public void AddBus(Func<IComponentContext, IBusControl> busFactory)
        {
            _builder.Register(busFactory)
                .As<IBusControl>()
                .As<IBus>()
                .SingleInstance();

            _builder.Register(GetCurrentSendEndpointProvider)
                .As<ISendEndpointProvider>()
                .InstancePerLifetimeScope();

            _builder.Register(GetCurrentPublishEndpoint)
                .As<IPublishEndpoint>()
                .InstancePerLifetimeScope();

            _builder.Register(context => context.Resolve<IBus>().CreateClientFactory())
                .As<IClientFactory>()
                .SingleInstance();
        }

        public void AddRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _builder.Register(context =>
            {
                var clientFactory = context.Resolve<IClientFactory>();

                return context.TryResolve(out ConsumeContext consumeContext)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            });
        }

        public void AddRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _builder.Register(context =>
            {
                var clientFactory = context.Resolve<IClientFactory>();

                return context.TryResolve(out ConsumeContext consumeContext)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            });
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            return context.Resolve<IBus>();
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IComponentContext context)
        {
            if (context.TryResolve(out ConsumeContext consumeContext))
                return consumeContext;

            return context.Resolve<IBus>();
        }
    }
}
