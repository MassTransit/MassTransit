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
namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using Pipeline;
    using Util;


    public class MultiTestConsumer
    {
        readonly IList<IConsumerConfigurator> _configures;
        internal readonly ReceivedMessageList _received;
        readonly TimeSpan _timeout;

        public MultiTestConsumer(TimeSpan timeout)
        {
            _timeout = timeout;
            _configures = new List<IConsumerConfigurator>();

            _received = new ReceivedMessageList(timeout);
        }

        public IReceivedMessageList Received
        {
            get { return _received; }
        }

        public TimeSpan Timeout
        {
            get { return _timeout; }
        }

        public ReceivedMessageList<T> Consume<T>()
            where T : class
        {
            var consumer = new Of<T>(this);
            var configure = new ConsumerConfigurator<T>(consumer);
            _configures.Add(configure);

            return consumer.Received;
        }

        public ConnectHandle Connect(IConsumePipeConnector bus)
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (IConsumerConfigurator configure in _configures)
                {
                    ConnectHandle handle = configure.Connect(bus);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (ConnectHandle handle in handles)
                    handle.Dispose();
                throw;
            }
        }

        public void Configure(IReceiveEndpointConfigurator configurator)
        {
            foreach (IConsumerConfigurator configure in _configures)
            {
                IInstanceConfigurator instanceConfigurator = configure.Configure(configurator);
            }
        }


        class ConsumerConfigurator<T> :
            IConsumerConfigurator
            where T : class
        {
            readonly Of<T> _consumer;

            public ConsumerConfigurator(Of<T> consumer)
            {
                _consumer = consumer;
            }

            public ConnectHandle Connect(IConsumePipeConnector bus)
            {
                return bus.ConnectInstance(_consumer);
            }

            public IInstanceConfigurator Configure(IReceiveEndpointConfigurator configurator)
            {
                return configurator.Instance(_consumer);
            }
        }


        interface IConsumerConfigurator
        {
            ConnectHandle Connect(IConsumePipeConnector bus);
            IInstanceConfigurator Configure(IReceiveEndpointConfigurator configurator);
        }


        class Of<T> :
            IConsumer<T>
            where T : class
        {
            readonly MultiTestConsumer _multiConsumer;
            readonly ReceivedMessageList<T> _received;

            public Of(MultiTestConsumer multiConsumer)
            {
                _multiConsumer = multiConsumer;
                _received = new ReceivedMessageList<T>(multiConsumer.Timeout);
            }

            public ReceivedMessageList<T> Received
            {
                get { return _received; }
            }

            public async Task Consume(ConsumeContext<T> context)
            {
                _received.Add(context);
                _multiConsumer._received.Add(context);
            }
        }
    }
}