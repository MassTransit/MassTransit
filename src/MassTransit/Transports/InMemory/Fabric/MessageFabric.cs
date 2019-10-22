// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using Internals.GraphValidation;


    public class MessageFabric :
        IMessageFabric
    {
        int _concurrencyLimit;
        readonly ConcurrentDictionary<string, IInMemoryExchange> _exchanges;
        readonly ConcurrentDictionary<string, IInMemoryQueue> _queues;

        public MessageFabric(int concurrencyLimit)
        {
            _concurrencyLimit = concurrencyLimit;

            _exchanges = new ConcurrentDictionary<string, IInMemoryExchange>(StringComparer.OrdinalIgnoreCase);
            _queues = new ConcurrentDictionary<string, IInMemoryQueue>(StringComparer.OrdinalIgnoreCase);
        }

        public Task Send(string exchangeName, InMemoryTransportMessage message)
        {
            var exchange = new InMemoryExchange(exchangeName);

            var deliveryContext = new InMemoryDeliveryContext(message);
            return exchange.Deliver(deliveryContext);
        }

        public void ExchangeDeclare(string name)
        {
            _exchanges.GetOrAdd(name, x => new InMemoryExchange(x));
        }

        public void QueueDeclare(string name, int concurrencyLimit)
        {
            _queues.GetOrAdd(name, x => new InMemoryQueue(x, concurrencyLimit == 0 ? _concurrencyLimit : concurrencyLimit));
        }

        public void ExchangeBind(string source, string destination)
        {
            if (source.Equals(destination))
                throw new ArgumentException("The source and destination exchange cannot be the same");

            var sourceExchange = _exchanges.GetOrAdd(source, x => new InMemoryExchange(x));

            var destinationExchange = _exchanges.GetOrAdd(destination, x => new InMemoryExchange(x));

            ValidateBinding(destinationExchange, sourceExchange);

            sourceExchange.Connect(destinationExchange);
        }

        public void QueueBind(string source, string destination)
        {
            var sourceExchange = _exchanges.GetOrAdd(source, x => new InMemoryExchange(x));

            var destinationQueue = _queues.GetOrAdd(destination, x => new InMemoryQueue(destination, _concurrencyLimit));

            ValidateBinding(destinationQueue, sourceExchange);

            sourceExchange.Connect(destinationQueue);
        }

        public IInMemoryQueue GetQueue(string name)
        {
            return _queues.GetOrAdd(name, x => new InMemoryQueue(x, _concurrencyLimit));
        }

        public IInMemoryExchange GetExchange(string name)
        {
            return _exchanges.GetOrAdd(name, x => new InMemoryExchange(x));
        }

        public int ConcurrencyLimit
        {
            set => _concurrencyLimit = value;
        }

        void ValidateBinding(IMessageSink<InMemoryTransportMessage> destination, IInMemoryExchange sourceExchange)
        {
            try
            {
                var graph = new DependencyGraph<IMessageSink<InMemoryTransportMessage>>(_exchanges.Count + 1);
                var exchanges = new List<IInMemoryExchange>(_exchanges.Values);
                foreach (var exchange in exchanges)
                {
                    var sinks = new List<IMessageSink<InMemoryTransportMessage>>(exchange.Sinks);
                    foreach (IMessageSink<InMemoryTransportMessage> sink in sinks)
                    {
                        graph.Add(sink, exchange);
                    }
                }

                graph.Add(destination, sourceExchange);

                graph.EnsureGraphIsAcyclic();
            }
            catch (CyclicGraphException exception)
            {
                throw new InvalidOperationException($"The exchange binding would create a cycle in the messaging fabric.", exception);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("messageFabric");
            foreach (var exchange in _exchanges)
            {
                var exchangeScope = scope.CreateScope("exchange");
                exchangeScope.Add("name", exchange.Key);
                foreach (IMessageSink<InMemoryTransportMessage> sink in exchange.Value.Sinks)
                {
                    exchangeScope.CreateScope("sink").Add("name", sink.ToString());
                }
            }

            foreach (var exchange in _queues)
            {
                var exchangeScope = scope.CreateScope("queue");
                exchangeScope.Add("name", exchange.Key);
            }
        }
    }
}
