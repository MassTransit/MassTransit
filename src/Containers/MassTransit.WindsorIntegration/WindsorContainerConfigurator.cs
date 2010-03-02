// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Configuration;
    using Saga;
    using Serialization;
    using Services.Subscriptions.Client;
    using Services.Subscriptions.Server;

    public static class WindsorContainerConfigurator
    {
        public static IObjectBuilder InitializeContainer(IWindsorContainer container)
        {
            var wob = new WindsorObjectBuilder(container.Kernel);

            container.Kernel.AddComponentInstance("kernel", typeof(IKernel), container.Kernel);
            container.Kernel.AddComponentInstance("objectBuilder", typeof(IObjectBuilder), wob);

            container.Register(
                Component.For<IObjectBuilder>()
                    .ImplementedBy<WindsorObjectBuilder>()
                    .LifeStyle.Singleton,
                // The subscription client
                Component.For<SubscriptionClient>()
                    .ImplementedBy<SubscriptionClient>()
                    .LifeStyle.Transient,
                // Message Serializers
                Component.For<BinaryMessageSerializer>()
                    .ImplementedBy<BinaryMessageSerializer>()
                    .LifeStyle.Singleton,
                Component.For<DotNotXmlMessageSerializer>()
                    .ImplementedBy<DotNotXmlMessageSerializer>()
                    .LifeStyle.Singleton,
                Component.For<XmlMessageSerializer>()
                    .ImplementedBy<XmlMessageSerializer>()
                    .LifeStyle.Singleton
                );

            ServiceBusConfigurator.Defaults(x => x.SetObjectBuilder(wob));

            return wob;
        }

        public static void RegisterInMemorySubscriptionRepository(IWindsorContainer container)
        {
            container.Kernel.Register(
                Component.For<ISubscriptionRepository>()
                    .ImplementedBy<InMemorySubscriptionRepository>()
                    .LifeStyle.Singleton
                );
        }

        public static void RegisterInMemorySagaRepository(IWindsorContainer container)
        {
            container.Kernel.Register(
                Component.For(typeof(ISagaRepository<>))
                    .ImplementedBy(typeof(InMemorySagaRepository<>))
                    .LifeStyle.Singleton
                );
        }
    }
}