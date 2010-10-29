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
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.Registration;
    using Internal;
    using Saga;
    using Services.Subscriptions;
    using Services.Subscriptions.Server;

    /// <summary>
    /// Facility to simplify the use of MT
    /// </summary>
    public abstract class MassTransitFacilityBase :
        AbstractFacility
    {

        protected override void Init()
        {
            RegisterBusDependencies();
        }



        /// <summary>
        /// Registers the in-memory subscription service so that all buses created in the same
        /// process share subscriptions
        /// </summary>
        protected void RegisterInMemorySubscriptionService()
        {
            Kernel.Register(
                Component.For<IEndpointSubscriptionEvent>().ImplementedBy<LocalSubscriptionService>().LifeStyle.Singleton,
                Component.For<SubscriptionPublisher>().ImplementedBy<SubscriptionPublisher>(),
                Component.For<SubscriptionConsumer>().ImplementedBy<SubscriptionConsumer>()
                );
        }

        protected void RegisterInMemorySubscriptionRepository()
        {
            Kernel.Register(
                Component.For<ISubscriptionRepository>().ImplementedBy<InMemorySubscriptionRepository>().LifeStyle.Singleton
                );
        }

        protected void RegisterInMemorySagaRepository()
        {
            Kernel.Register(
                Component.For(typeof(ISagaRepository<>)).ImplementedBy(typeof(InMemorySagaRepository<>)).LifeStyle.Singleton
                );
        }

        /// <summary>
        /// Registers the types used by the service bus internally and as part of the container.
        /// These are typically items that are not swapped based on the container implementation
        /// </summary>
        protected void RegisterBusDependencies()
        {
            Kernel.Register(
                Component.For<IObjectBuilder>().ImplementedBy<WindsorObjectBuilder>().LifeStyle.Singleton
                );

            //we are expecting SM to auto-resolve
            // SubscriptionClient
            // InitiateSagaMessageSink<,>
            // OrchestrateSagaMessageSink<,>)
            // InitiateSagaStateMachineSink<,>)
            // OrchestrateSagaStateMachineSink<,>)
        }
    }
}