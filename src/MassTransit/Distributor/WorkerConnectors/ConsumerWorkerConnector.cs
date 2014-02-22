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
namespace MassTransit.Distributor.WorkerConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Sinks;
    using Saga;
    using SubscriptionConnectors;
    using Subscriptions;
    using Util;


    public interface ConsumerWorkerConnector
    {
        Type MessageType { get; }

        UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IWorker worker);
    }


    public class ConsumerWorkerConnector<T> :
        WorkerConnector
        where T : class
    {
        readonly IEnumerable<ConsumerWorkerConnector> _connectors;
        readonly IConsumerFactory<T> _consumerFactory;
        readonly ReferenceFactory _referenceFactory;

        public ConsumerWorkerConnector(ReferenceFactory referenceFactory, IConsumerFactory<T> consumerFactory)
        {
            _referenceFactory = referenceFactory;
            _consumerFactory = consumerFactory;

            Type[] interfaces = typeof(T).GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                throw new ConfigurationException("A saga cannot be registered as a consumer");

            if (interfaces.Implements(typeof(InitiatedBy<>))
                || interfaces.Implements(typeof(Orchestrates<>))
                || interfaces.Implements(typeof(Observes<,>)))
                throw new ConfigurationException("InitiatedBy, Orchestrates, and Observes can only be used with sagas");

            _connectors = ConsumesContext()
                .Concat(ConsumesAll())
                .Distinct((x, y) => x.MessageType == y.MessageType)
                .ToList();
        }

        public ISubscriptionReference Connect(IInboundPipelineConfigurator configurator, IWorker worker)
        {
            return _referenceFactory(_connectors.Select(x => x.Connect(configurator, worker))
                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x()));
        }

        IEnumerable<ConsumerWorkerConnector> ConsumesContext()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesContextTypes()
                .Select(CreateContextConnector);
        }

        ConsumerWorkerConnector CreateContextConnector(MessageInterfaceType x)
        {
            return (ConsumerWorkerConnector)
                FastActivator.Create(typeof(ContextConsumerWorkerConnector<,>),
                    new[] {typeof(T), x.MessageType}, new object[] {_consumerFactory});
        }

        IEnumerable<ConsumerWorkerConnector> ConsumesAll()
        {
            return MessageInterfaceTypeReflector<T>.GetConsumesAllTypes()
                .Select(CreateConnector);
        }

        ConsumerWorkerConnector CreateConnector(MessageInterfaceType x)
        {
            return (ConsumerWorkerConnector)
                FastActivator.Create(typeof(ConsumerWorkerConnector<,>),
                    new[] {typeof(T), x.MessageType}, new object[] {_consumerFactory});
        }
    }


    public class ConsumerWorkerConnector<TConsumer, TMessage> :
        ConsumerWorkerConnectorImpl<TConsumer, TMessage>
        where TMessage : class
        where TConsumer : class, Consumes<TMessage>.All
    {
        public ConsumerWorkerConnector(IConsumerFactory<TConsumer> consumerFactory)
            : base(consumerFactory)
        {
        }

        protected override IPipelineSink<IConsumeContext<TMessage>> GetConsumerSink(
            IConsumerFactory<TConsumer> consumerFactory)
        {
            return new ConsumerMessageSink<TConsumer, TMessage>(consumerFactory);
        }
    }
}