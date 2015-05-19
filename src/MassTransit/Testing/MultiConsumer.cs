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

namespace MassTransit.Testing
{
    using System.Collections.Generic;

#if NET40
    public class MultiConsumer
    {
        readonly IList<IConfigure> _configures;
        internal readonly ReceivedMessageListImpl _received;

        public MultiConsumer()
        {
            _configures = new List<IConfigure>();
            _received = new ReceivedMessageListImpl();
        }

        public ReceivedMessageList Received
        {
            get { return _received; }
        }

        public ReceivedMessageList<T> Consume<T>()
            where T : class
        {
            var consumer = new Of<T>(this);
            var configure = new Configure<T>(consumer);
            _configures.Add(configure);

            return consumer.Received;
        }

        public UnsubscribeAction Subscribe(IServiceBus bus)
        {
            UnsubscribeAction result = () => true;
            foreach (IConfigure configure in _configures)
            {
                result += configure.Subscribe(bus);
            }

            return result;
        }
    }

    interface IConfigure
    {
        UnsubscribeAction Subscribe(IServiceBus bus);
    }

    class Configure<T> :
        IConfigure
        where T : class
    {
        readonly Of<T> _consumer;

        public Configure(Of<T> consumer)
        {
            _consumer = consumer;
        }

        public UnsubscribeAction Subscribe(IServiceBus bus)
        {
            return bus.SubscribeInstance(_consumer);
        }
    }

    class Of<T> :
        Consumes<T>.Context
        where T : class
    {
        readonly MultiConsumer _multiConsumer;
        readonly ReceivedMessageListImpl<T> _received;

        public Of(MultiConsumer multiConsumer)
        {
            _multiConsumer = multiConsumer;
            _received = new ReceivedMessageListImpl<T>();
        }

        public ReceivedMessageList<T> Received
        {
            get { return _received; }
        }

        public void Consume(IConsumeContext<T> message)
        {
            _received.Add(new ReceivedMessageImpl<T>(message));
            _multiConsumer._received.Add(new ReceivedMessageImpl<T>(message));
        }
    }

#endif
}