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
namespace MassTransit.Testing.Scenarios
{
    using Magnum.Extensions;
    using TestDecorators;
    using Transports;


    public abstract class EndpointTestScenarioImpl :
        EndpointTestScenario
    {
        readonly PublishedMessageListImpl _published;
        readonly ReceivedMessageListImpl _received;
        readonly SentMessageListImpl _sent;
        readonly ReceivedMessageListImpl _skipped;
        bool _disposed;

        protected EndpointTestScenarioImpl(IEndpointFactory endpointFactory)
        {
            _received = new ReceivedMessageListImpl();
            _sent = new SentMessageListImpl();
            _skipped = new ReceivedMessageListImpl();
            _published = new PublishedMessageListImpl();

            ServiceBusFactory.ConfigureDefaultSettings(x =>
            {
                x.SetConcurrentConsumerLimit(4);
                x.SetConcurrentReceiverLimit(1);
                x.SetReceiveTimeout(50.Milliseconds());
                x.EnableAutoStart();
            });
        }

        public SentMessageList Sent
        {
            get { return _sent; }
        }

        public ReceivedMessageList Skipped
        {
            get { return _skipped; }
        }

        public abstract IBus InputBus { get; }

        public abstract ISendEndpoint OutputBus { get; }

        public PublishedMessageList Published
        {
            get { return _published; }
        }

        public ReceivedMessageList Received
        {
            get { return _received; }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void AddSent(SentMessage message)
        {
            _sent.Add(message);
        }

        public void AddPublished(PublishedMessage message)
        {
            _published.Add(message);
        }

        public void AddReceived(ReceivedMessage message)
        {
            _received.Add(message);

            _skipped.Remove(message);
        }

        public void AddSkipped(ReceivedMessage message)
        {
            _skipped.Add(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _sent.Dispose();
                _received.Dispose();

                ServiceBusFactory.ConfigureDefaultSettings(x => x.SetEndpointCache(null));
            }

            _disposed = true;
        }

        public virtual IServiceBus GetDecoratedBus(IServiceBus bus)
        {
            return new ServiceBusTestDecorator(bus, this);
        }
    }
}