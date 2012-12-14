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
    using EndpointConfigurators;
    using Magnum.Extensions;
    using Pipeline.Configuration;
    using Transports;
    using Transports.RabbitMq;
    using Transports.RabbitMq.Configuration.Configurators;


    public static class RabbitMqServiceBusExtensions
    {
        /// <summary>
        /// Returns the endpoint for the specified message type using the default
        /// exchange/queue convention for naming
        /// </summary>
        /// <typeparam name="TMessage">The message type to convert to a URI</typeparam>
        /// <param name="bus">The bus instance used to resolve the endpoint</param>
        /// <returns>The IEndpoint instance, resolved from the service bus</returns>
        public static IEndpoint GetEndpoint<TMessage>(this IServiceBus bus)
            where TMessage : class
        {
            return GetEndpoint(bus, typeof(TMessage));
        }

        /// <summary>
        /// Returns the endpoint for the specified message type using the default
        /// exchange/queue convention for naming.
        /// 
        /// TODO: FIX!!!
        /// 
        /// </summary>
        /// <param name="bus">The bus instance used to resolve the endpoint</param>
        /// <param name="messageType">The message type to convert to a URI</param>
        /// <returns>The IEndpoint instance, resolved from the service bus</returns>
        public static IEndpoint GetEndpoint(this IServiceBus bus, Type messageType)
        {
            var inboundTransport = bus.Endpoint.InboundTransport as InboundRabbitMqTransport;
            if (inboundTransport == null)
            {
                throw new ArgumentException(
                    "The bus must be receiving from a RabbitMQ endpoint to convert message types to endpoints.");
            }

            var inputAddress = inboundTransport.Address.CastAs<IRabbitMqEndpointAddress>();

            IMessageNameFormatter messageNameFormatter = inboundTransport.MessageNameFormatter;

            MessageName messageName = messageNameFormatter.GetMessageName(messageType);

            IRabbitMqEndpointAddress address = inputAddress.ForQueue(messageName.ToString());

            return bus.GetEndpoint(address.Uri);
        }

        /// <summary>
        /// <see cref="UseRabbitMq{T}(T,Action{RabbitMqTransportFactoryConfigurator})"/>
        /// </summary>
        public static void UseRabbitMq(this EndpointFactoryConfigurator configurator)
        {
            UseRabbitMq(configurator, x => { });
        }

        /// <summary>
        /// <para>This method specifies that the service bus under configuration is to 
        /// use RabbitMQ for message queueing. See http://readthedocs.org/docs/masstransit/en/latest/configuration/transports/rabbitmq.html.
        /// This method also calls <see cref="SerializerConfigurationExtensions.UseJsonSerializer{T}"/>.</para>
        /// 
        /// </summary>
        /// <typeparam name="T">configurator type param</typeparam>
        /// <param name="configurator">configurator instance</param>
        /// <param name="configureFactory">custom action used to call APIs on the configurator</param>
        /// <returns>the configurator instance</returns>
        public static void UseRabbitMq(this EndpointFactoryConfigurator configurator,
            Action<RabbitMqTransportFactoryConfigurator> configureFactory)
        {
            var transportFactoryConfigurator = new RabbitMqTransportFactoryConfiguratorImpl();

            configureFactory(transportFactoryConfigurator);

            configurator.AddTransportFactory(transportFactoryConfigurator.Build);

            configurator.UseJsonSerializer();
        }

        /// <summary>
        /// Specifies that RabbitMQ should be added as a transport for the service bus.
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static void UseRabbitMq(this ServiceBusConfigurator configurator)
        {
            UseRabbitMq(configurator, x => { });
        }

        /// <summary>
        /// Specifies that RabbitMQ should be added as a transport for the service bus. Includes a callback
        /// that can configure additional settings on the transport, such as SSL.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configureFactory"></param>
        /// <returns></returns>
        public static void UseRabbitMq(this ServiceBusConfigurator configurator,
            Action<RabbitMqTransportFactoryConfigurator> configureFactory)
        {
            configurator.SetSubscriptionObserver((bus, coordinator) => new RabbitMqSubscriptionBinder(bus));

            var busConfigurator = new PostCreateBusBuilderConfigurator(bus =>
                {
                    var interceptorConfigurator = new OutboundMessageInterceptorConfigurator(bus.OutboundPipeline);

                    // make sure we publish correctly through this interceptor; works on the outgoing pipeline
                    interceptorConfigurator.Create(new PublishEndpointInterceptor(bus));
                });

            configurator.AddBusConfigurator(busConfigurator);

            UseRabbitMq((EndpointFactoryConfigurator)configurator, configureFactory);
        }
    }
}