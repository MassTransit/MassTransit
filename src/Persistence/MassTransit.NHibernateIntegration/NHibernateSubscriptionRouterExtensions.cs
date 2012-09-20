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
    using NHibernate;
    using NHibernateIntegration.Subscriptions;
    using SubscriptionConfigurators;
    using Subscriptions.Coordinator;

    public static class NHibernateSubscriptionRouterExtensions
    {
        /// <summary>
        /// Specify subscription storage for the bus instance using NHibernate
        /// </summary>
        /// <param name="sessionFactory">The session factory for the storage</param>
        public static void UseNHibernateSubscriptionStorage(this ServiceBusConfigurator configurator,
            ISessionFactory sessionFactory)
        {
            Func<SubscriptionStorage> factoryMethod = () => new NHibernateSubscriptionStorage(sessionFactory);

            var builderConfigurator = new SubscriptionRouterBuilderConfiguratorImpl(
                x => x.UseSubscriptionStorage(factoryMethod));

            configurator.AddSubscriptionRouterConfigurator(builderConfigurator);
        }
    }
}