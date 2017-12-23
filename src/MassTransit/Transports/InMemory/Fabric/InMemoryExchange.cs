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
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class InMemoryExchange :
        IInMemoryExchange
    {
        readonly HashSet<IMessageSink<InMemoryTransportMessage>> _sinks;

        public string Name { get; }

        public InMemoryExchange(string name)
        {
            Name = name;
            _sinks = new HashSet<IMessageSink<InMemoryTransportMessage>>();
        }

        public async Task Deliver(DeliveryContext<InMemoryTransportMessage> context)
        {
            foreach (IMessageSink<InMemoryTransportMessage> sink in _sinks)
            {
                if (context.WasAlreadyDelivered(sink))
                    continue;

                await sink.Deliver(context).ConfigureAwait(false);

                context.Delivered(sink);
            }
        }

        public void Connect(IMessageSink<InMemoryTransportMessage> sink)
        {
            _sinks.Add(sink);
        }

        public IEnumerable<IMessageSink<InMemoryTransportMessage>> Sinks => _sinks;

        public Task Send(InMemoryTransportMessage message)
        {
            var deliveryContext = new InMemoryDeliveryContext(message);
            return Deliver(deliveryContext);
        }

        public override string ToString()
        {
            return $"Exchange({Name})";
        }
    }
}