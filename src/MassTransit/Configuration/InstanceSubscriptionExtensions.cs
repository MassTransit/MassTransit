// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using ConsumeConfigurators;
    using ConsumeConnectors;


    /// <summary>
    /// Extensions for subscribing object instances.
    /// </summary>
    public static class InstanceSubscriptionExtensions
    {
        /// <summary>
        /// Subscribes an object instance to the bus
        /// </summary>
        /// <param name="configurator">Service Bus Service Configurator 
        /// - the item that is passed as a parameter to
        /// the action that is calling the configurator.</param>
        /// <param name="instance">The instance to subscribe.</param>
        /// <returns>An instance subscription configurator.</returns>
        public static IInstanceConfigurator Instance(this IReceiveEndpointConfigurator configurator, object instance)
        {
            var instanceConfigurator = new InstanceConfigurator(instance);

            configurator.AddEndpointSpecification(instanceConfigurator);

            return instanceConfigurator;
        }

        /// <summary>
        /// Connects any consumers for the object to the message dispatcher
        /// </summary>
        /// <param name="bus">The service bus to configure</param>
        /// <param name="instance"></param>
        /// <returns>The unsubscribe action that can be called to unsubscribe the instance
        /// passed as an argument.</returns>
        public static ConnectHandle ConnectInstance(this IBus bus, object instance)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (instance == null)
                throw new ArgumentNullException("instance");

            InstanceConnector connector = InstanceConnectorCache.GetInstanceConnector(instance.GetType());

            return connector.Connect(bus, instance);
        }

        /// <summary>
        /// Connects any consumers for the object to the message dispatcher
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="bus">The service bus instance to call this method on.</param>
        /// <param name="instance">The instance to subscribe.</param>
        /// <returns>The unsubscribe action that can be called to unsubscribe the instance
        /// passed as an argument.</returns>
        public static ConnectHandle ConnectInstance<T>(this IBus bus, T instance)
            where T : class, IConsumer
        {
            InstanceConnector connector = InstanceConnectorCache.GetInstanceConnector<T>();

            return connector.Connect(bus, instance);
        }
    }
}