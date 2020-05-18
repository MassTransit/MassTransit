// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System.Threading.Tasks;
    using Apache.NMS;
    using GreenPipes.Agents;
    using GreenPipes.Caching;
    using Util;
    using Transports;


    public class MessageProducerCache :
        Agent
    {
        public delegate Task<IMessageProducer> MessageProducerFactory(IDestination destination);


        readonly IIndex<IDestination, CachedMessageProducer> _index;
        readonly GreenCache<CachedMessageProducer> _cache;

        public MessageProducerCache()
        {
            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);
            _cache = new GreenCache<CachedMessageProducer>(cacheSettings);
            _cache.Connect(new CloseAndDisposeOnRemoveObserver());

            _index = _cache.AddIndex("destination", x => x.Destination);
        }

        public async Task<IMessageProducer> GetMessageProducer(IDestination key, MessageProducerFactory factory)
        {
            var messageProducer = await _index.Get(key, x => GetMessageProducerFromFactory(x, factory)).ConfigureAwait(false);

            return messageProducer;
        }

        async Task<CachedMessageProducer> GetMessageProducerFromFactory(IDestination destination, MessageProducerFactory factory)
        {
            var messageProducer = await factory(destination).ConfigureAwait(false);

            return new CachedMessageProducer(destination, messageProducer);
        }

        protected override Task StopAgent(StopContext context)
        {
            foreach (var producer in _cache.GetAll())
            {
                producer.Dispose();
            }

            _cache.Clear();

            return TaskUtil.Completed;
        }


        class CloseAndDisposeOnRemoveObserver :
            ICacheValueObserver<CachedMessageProducer>
        {
            public void ValueAdded(INode<CachedMessageProducer> node, CachedMessageProducer value)
            {
            }

            public void ValueRemoved(INode<CachedMessageProducer> node, CachedMessageProducer value)
            {
                value.Close();
                value.Dispose();
            }

            public void CacheCleared()
            {
            }
        }
    }
}