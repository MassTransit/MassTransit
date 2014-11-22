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
namespace MassTransit.TestFramework
{
    using System;
    using System.Threading.Tasks;
    using EndpointConfigurators;
    using Logging;
    using MassTransit.Transports;
    using NUnit.Framework;
    using Testing;
    using Testing.TestDecorators;


    [TestFixture]
    public class InMemoryTestFixture :
        LocalBusTestFixture
    {
        static readonly ILog _log = Logger.Get<InMemoryTestFixture>();
        IBusControl _localBus;
        Uri _inputQueueAddress;
        SendEndpointTestDecorator _inputQueueSendEndpoint;
        InMemoryTransportCache _transportCache;
        readonly Lazy<Task<ISendEndpoint>> _localBusSendEndpoint;

        public InMemoryTestFixture()
        {
            _inputQueueAddress = new Uri("loopback://localhost/input_queue");
            _localBusSendEndpoint = new Lazy<Task<ISendEndpoint>>(() => _localBus.GetSendEndpoint(_localBus.InputAddress));
        }

        protected ISendEndpoint InputQueueSendEndpoint
        {
            get { return _inputQueueSendEndpoint; }
        }

        protected Task<ISendEndpoint> LocalBusSendEndpoint
        {
            get { return _localBusSendEndpoint.Value; }
        }

        protected MessageSentList Sent
        {
            get { return _inputQueueSendEndpoint.Sent; }
        }

        protected Uri LocalBusAddress
        {
            get { return _localBus.InputAddress; }
        }

        protected Uri InputQueueAddress
        {
            get { return _inputQueueAddress; }
            set
            {
                if (LocalBus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _inputQueueAddress = value;
            }
        }

        protected override IBus LocalBus
        {
            get { return _localBus; }
        }

        [TestFixtureSetUp]
        public void SetupInMemoryTestFixture()
        {
            _transportCache = new InMemoryTransportCache();

            _localBus = CreateLocalBus(_transportCache);

            _localBus.Start(TestCancellationToken).Wait(TestTimeout);

            ISendEndpoint sendEndpoint = _localBus.GetSendEndpoint(_inputQueueAddress).Result;

            _inputQueueSendEndpoint = new SendEndpointTestDecorator(sendEndpoint, TestTimeout);
        }

        [TestFixtureTearDown]
        public void TearDownInMemoryTestFixture()
        {
            try
            {
                if (_localBus != null)
                    _localBus.Stop().Wait(TestTimeout);
            }
            catch (AggregateException ex)
            {
                _log.Error("LocalBus Stop Failed: ", ex);
                throw;
            }

            try
            {
                if (_transportCache != null)
                    _transportCache.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error("TransportCache.Dispose failed", ex);
                throw;
            }

            _localBus = null;
        }

        protected virtual void ConfigureLocalReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateLocalBus(InMemoryTransportCache transportCache)
        {
            return ServiceBusFactory.New(x => x.InMemory(), x =>
            {
                x.SetTransportProvider(transportCache);

                ConfigureLocalBus(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Log(Console.Out, async context => string.Format("Received (input_queue): {0}, Types = ({1})",
                        context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A"),
                        string.Join(",", context.SupportedMessageTypes)));

                    ConfigureLocalReceiveEndpoint(e);
                });
            });
        }

        protected virtual void ConfigureLocalBus(IInMemoryServiceBusFactoryConfigurator configurator)
        {
        }
    }
}