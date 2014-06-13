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
namespace MassTransit
{
    using System;
    using Magnum;
    using Pipeline;
    using Pipeline.Sinks;
	using SubscriptionConfigurators;
	using SubscriptionConnectors;
	using Util;

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
		[NotNull]
		public static InstanceSubscriptionConfigurator Instance(
			[NotNull] this SubscriptionBusServiceConfigurator configurator,
            [NotNull] object instance, IMessageRetryPolicy retryPolicy = null)
		{
			var instanceConfigurator = new InstanceSubscriptionConfiguratorImpl(instance, retryPolicy ?? Retry.None);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(instanceConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return instanceConfigurator;
		}


		/// <summary>
		/// Connects any consumers for the component to the message dispatcher
		/// </summary>
		/// <param name="bus">The service bus to configure</param>
		/// <param name="instance"></param>
		/// <returns>The unsubscribe action that can be called to unsubscribe the instance
		/// passed as an argument.</returns>
		public static UnsubscribeAction SubscribeInstance([NotNull] this IServiceBus bus, [NotNull] object instance)
		{
			Guard.AgainstNull(instance, "instance", "A null instance cannot be subscribed");

			InstanceConnector connector = InstanceConnectorCache.GetInstanceConnector(instance.GetType());

		    throw new NotImplementedException();

			//return bus.Configure(x => connector.Connect(x, instance));
		}

	    /// <summary>
	    /// Connects any consumers for the component to the message dispatcher
	    /// </summary>
	    /// <typeparam name="T">The consumer type</typeparam>
	    /// <param name="bus">The service bus instance to call this method on.</param>
	    /// <param name="instance">The instance to subscribe.</param>
	    /// <param name="retryPolicy"></param>
	    /// <returns>The unsubscribe action that can be called to unsubscribe the instance
	    /// passed as an argument.</returns>
	    public static ConnectHandle SubscribeInstance<T>([NotNull] this IServiceBus bus, [NotNull] T instance, IMessageRetryPolicy retryPolicy = null)
			where T : class, IConsumer
		{
			Guard.AgainstNull(instance, "instance", "A null instance cannot be subscribed");

			InstanceConnector connector = InstanceConnectorCache.GetInstanceConnector<T>();

		    return connector.Connect(bus.InboundPipe, instance, retryPolicy ?? Retry.None);
		}
	}
}