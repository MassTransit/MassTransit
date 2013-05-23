// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.BusConfigurators
{
    using System;
    using Builders;
    using Configuration;
    using EndpointConfigurators;
    using SubscriptionConfigurators;

    /// <summary>
    /// <para>The configurator to call methods on, as well as extension methods on,
    /// in order to configure your service bus. The configuration
    /// goes a lot by convention, but this interface allows you to configure
    /// almost any aspect of the bus.</para>
    /// 
    /// <para>
    /// Documentation is at http://readthedocs.org/docs/masstransit/en/latest/configuration/index.html
    /// </para>
    /// </summary>
    public interface ServiceBusConfigurator :
        EndpointFactoryConfigurator
    {
        /// <summary>
        /// Specifies the builder factory to use when the service is invoked
        /// </summary>
        /// <param name="builderFactory"></param>
        void UseBusBuilder(Func<BusSettings, BusBuilder> builderFactory);

        /// <summary>
        /// Adds a configurator to the subscription coordinator builder
        /// </summary>
        /// <param name="configurator"></param>
        void AddSubscriptionRouterConfigurator(SubscriptionRouterBuilderConfigurator configurator);

        /// <summary>
        /// Adds a configurator for the service bus builder to the configurator
        /// </summary>
        /// <param name="configurator"></param>
        void AddBusConfigurator(BusBuilderConfigurator configurator);

        /// <summary>
        /// Specify the endpoint from which messages should be read
        /// </summary>
        /// <param name="uri">
        /// The uri of the endpoint that this bus should
        /// receive message from.
        /// </param>
        void ReceiveFrom(Uri uri);

        /// <summary>
        /// Sets the network key for subscriptions
        /// </summary>
        /// <param name="network"></param>
        void SetNetwork(string network);

        /// <summary>
        /// Disable the performance counters
        /// </summary>
        void DisablePerformanceCounters();

        /// <summary>
        /// Specifies an action to call before a message is consumed. Implementors
        /// should take care to not remove previously set actions so that multiple
        /// calls to this method generates calls to all those action parameters.
        /// </summary>
        /// <param name="beforeConsume">The action to run before consumption.</param>
        void BeforeConsumingMessage(Action beforeConsume);

        /// <summary>
        /// Specifies an action to call after a message is consumed. Implementors
        /// should take care to not remove previously set actions so that multiple
        /// calls to this method generates calls to all those action parameters.
        /// </summary>
        /// <param name="afterConsume">The action to run after consumption</param>
        void AfterConsumingMessage(Action afterConsume);
    }
}