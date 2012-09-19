// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using BusConfigurators;
    using SubscriptionConfigurators;
    using Subscriptions.Coordinator;

    public static class SubscriptionRouterConfiguratorExtensions
    {
        public static void SetSubscriptionObserver(this ServiceBusConfigurator configurator,
            Func<IServiceBus, SubscriptionRouter, SubscriptionObserver>
                observerFactory)
        {
            var coordinatorConfigurator =
                new SubscriptionRouterBuilderConfiguratorImpl(x => { x.SetObserverFactory(observerFactory); });

            configurator.AddSubscriptionRouterConfigurator(coordinatorConfigurator);
        }

        public static void AddSubscriptionObserver(this ServiceBusConfigurator configurator,
            Func<IServiceBus, SubscriptionRouter, SubscriptionObserver>
                observerFactory)
        {
            var coordinatorConfigurator =
                new SubscriptionRouterBuilderConfiguratorImpl(x => { x.AddObserverFactory(observerFactory); });

            configurator.AddSubscriptionRouterConfigurator(coordinatorConfigurator);
        }

        /// <summary>
        /// Specify a custom subscription storage for the bus instance
        /// </summary>
        /// <param name="subscriptionStorageFactory">Factory method for the subscription storage</param>
        public static void UseSubscriptionStorage(this ServiceBusConfigurator configurator,
            Func<SubscriptionStorage> subscriptionStorageFactory)
        {
            var builderConfigurator = new SubscriptionRouterBuilderConfiguratorImpl(
                x => x.UseSubscriptionStorage(subscriptionStorageFactory));

            configurator.AddSubscriptionRouterConfigurator(builderConfigurator);
        }
    }
}