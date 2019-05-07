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
namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.Registration;
    using ScopeProviders;
    using Scoping;


    public class WindsorContainerRegistrationConfigurator :
        RegistrationConfigurator,
        IWindsorContainerConfigurator
    {
        readonly IWindsorContainer _container;

        public WindsorContainerRegistrationConfigurator(IWindsorContainer container)
            : base(new WindsorContainerRegistrar(container))
        {
            _container = container;

            container.Register(
                Component.For<ScopedConsumeContextProvider>().LifestyleScoped(),
                Component.For<ConsumeContext>().UsingFactoryMethod(kernel => kernel.Resolve<ScopedConsumeContextProvider>().GetContext()).LifestyleScoped(),
                Component.For<IConsumerScopeProvider>().ImplementedBy<WindsorConsumerScopeProvider>().LifestyleTransient(),
                Component.For<IConfigurationServiceProvider>()
                    .ImplementedBy<WindsorConfigurationServiceProvider>()
                    .LifestyleSingleton(),
                Component.For<ISagaRepositoryFactory>()
                    .ImplementedBy<WindsorSagaRepositoryFactory>()
                    .LifestyleSingleton(),
                Component.For<IRegistrationConfigurator>()
                    .Instance(this)
                    .LifestyleSingleton(),
                Component.For<MassTransit.IRegistration>()
                    .UsingFactoryMethod(provider => CreateRegistration(provider.Resolve<IConfigurationServiceProvider>()))
                    .LifestyleSingleton()
            );
        }

        IConsumerScopeProvider CreateConsumerScopeProvider(IKernel kernel)
        {
            return new WindsorConsumerScopeProvider(kernel);
        }

        public IWindsorContainer Container => _container;

        public void AddBus(Func<IKernel, IBusControl> busFactory)
        {
            _container.Register(
                Component.For<IBusControl>()
                    .Forward<IBus>()
                    .UsingFactoryMethod(busFactory).LifestyleSingleton(),
                Component.For<ISendEndpointProvider>()
                    .UsingFactoryMethod(GetCurrentSendEndpointProvider)
                    .LifestyleTransient(),
                Component.For<IPublishEndpoint>()
                    .UsingFactoryMethod(GetCurrentPublishEndpoint)
                    .LifestyleTransient(),
                Component.For<IClientFactory>()
                    .UsingFactoryMethod(kernel => kernel.Resolve<IBus>().CreateClientFactory(default))
                    .LifestyleSingleton()
            );
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IKernel context)
        {
            var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
            return (currentScope != null)
                ? context.Resolve<ConsumeContext>()
                : (ISendEndpointProvider)context.Resolve<IBus>();
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IKernel context)
        {
            var currentScope = CallContextLifetimeScope.ObtainCurrentScope();
            return (currentScope != null)
                ? context.Resolve<ConsumeContext>()
                : (IPublishEndpoint)context.Resolve<IBus>();
        }
    }
}
