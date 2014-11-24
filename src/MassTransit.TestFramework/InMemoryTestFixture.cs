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
    using EndpointConfigurators;
    using Logging;
    using MassTransit.Transports;
    using NUnit.Framework;
    using Testing;
    using Testing.TestDecorators;


    [TestFixture]
    public class InMemoryTestFixture :
        BusTestFixture
    {
        static readonly ILog _log = Logger.Get<InMemoryTestFixture>();

        IBusControl _bus;
        Uri _inputQueueAddress;
        ISendEndpoint _inputQueueSendEndpoint;
        readonly InMemoryTransportCache _transportCache;
        ISendEndpoint _busSendEndpoint;
        readonly TestSendObserver _sendObserver;

        public InMemoryTestFixture()
        {
            _transportCache = new InMemoryTransportCache();
            _sendObserver = new TestSendObserver(TestTimeout);

            _inputQueueAddress = new Uri("loopback://localhost/input_queue");
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint
        {
            get { return _inputQueueSendEndpoint; }
        }

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint
        {
            get { return _busSendEndpoint; }
        }

        protected ISentMessageList Sent
        {
            get { return _sendObserver.Messages; }
        }

        protected Uri BusAddress
        {
            get { return _bus.InputAddress; }
        }

        protected Uri InputQueueAddress
        {
            get { return _inputQueueAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _inputQueueAddress = value;
            }
        }

        protected override IBus Bus
        {
            get { return _bus; }
        }

        [TestFixtureSetUp]
        public void SetupInMemoryTestFixture()
        {
            _bus = CreateBus(_transportCache);

            _bus.Start(TestCancellationToken).Wait(TestTimeout);

            _busSendEndpoint = _bus.GetSendEndpoint(_bus.InputAddress).Result;
            _busSendEndpoint.Connect(_sendObserver);

            _inputQueueSendEndpoint = _bus.GetSendEndpoint(_inputQueueAddress).Result;
            _inputQueueSendEndpoint.Connect(_sendObserver);
        }

        [TestFixtureTearDown]
        public void TearDownInMemoryTestFixture()
        {
            try
            {
                if (_bus != null)
                    _bus.Stop().Wait(TestTimeout);
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

            _bus = null;
        }

        protected virtual void ConfigureBus(IInMemoryServiceBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateBus(InMemoryTransportCache transportCache)
        {
            return ServiceBusFactory.New(x => x.InMemory(), x =>
            {
                x.SetTransportProvider(transportCache);

                ConfigureBus(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Log(Console.Out, async context => string.Format("Received (input_queue): {0}, Types = ({1})",
                        context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A"),
                        string.Join(",", context.SupportedMessageTypes)));

                    ConfigureInputQueueEndpoint(e);
                });
            });
        }
    }
}