// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConnectors
{
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Magnum.Reflection;
    using Pipeline;
    using Pipeline.Sinks;
    using Util;


    /// <summary>
    ///     Interface implemented by objects that tie an inbound pipeline together with
    ///     consumers (by means of calling a consumer factory).
    /// </summary>
    public interface ConsumerConnector
    {
        ConnectHandle Connect<TConsumer>(IInboundMessagePipe pipe, IAsyncConsumerFactory<TConsumer> consumerFactory,
            IMessageRetryPolicy retryPolicy)
            where TConsumer : class;
    }


    public class ConsumerConnector<T> :
        ConsumerConnector
        where T : class
    {
        readonly IEnumerable<ConsumerMessageConnector> _connectors;

        public ConsumerConnector()
        {
            if (TypeMetadataCache<T>.HasSagaInterfaces)
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            _connectors = Consumes().ToList();
        }

        public IEnumerable<ConsumerMessageConnector> Connectors
        {
            get { return _connectors; }
        }

        public ConnectHandle Connect<TConsumer>(IInboundMessagePipe pipe,
            IAsyncConsumerFactory<TConsumer> consumerFactory, IMessageRetryPolicy retryPolicy)
            where TConsumer : class
        {
            return new MultipleConnectHandle(_connectors.Select(x => x.Connect(pipe, consumerFactory, retryPolicy)));
        }


        static IEnumerable<ConsumerSubscriptionConnector> ConsumesContext()
        {
            yield break;
//            return ConsumerMetadataCache<T>.ConsumerTypes.Select(CreateContextConnector);
        }


        static IEnumerable<ConsumerMessageConnector> Consumes()
        {
            return ConsumerMetadataCache<T>.ConsumerTypes.Select(x => x.GetConsumerMessageConnector());
        }

        static ConsumerSubscriptionConnector CreateContextConnector(MessageInterfaceType x)
        {
            return (ConsumerSubscriptionConnector)
                FastActivator.Create(typeof(ContextConsumerSubscriptionConnector<,>),
                    new[] {typeof(T), x.MessageType});
        }

        static IEnumerable<ConsumerSubscriptionConnector> ConsumesAll()
        {
            return ConsumerMetadataCache<T>.MessageConsumerTypes.Select(CreateConnector);
        }

        static ConsumerSubscriptionConnector CreateConnector(MessageInterfaceType x)
        {
            return (ConsumerSubscriptionConnector)
                FastActivator.Create(typeof(ConsumerSubscriptionConnector<,>),
                    new[] {typeof(T), x.MessageType});
        }
    }
}