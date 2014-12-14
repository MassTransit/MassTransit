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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Logging;
    using MassTransit.Testing;
    using MassTransit.Testing.TestDecorators;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class RabbitMqTestFixture :
        BusTestFixture
    {
        static readonly ILog _log = Logger.Get<RabbitMqTestFixture>();
        IBusControl _bus;
        Uri _inputQueueAddress;
        ISendEndpoint _inputQueueSendEndpoint;
        ISendEndpoint _busSendEndpoint;
        readonly TestSendObserver _sendObserver;
        Uri _localHostUri;
        BusHandle _busHandle;

        public RabbitMqTestFixture()
        {
            _localHostUri = new Uri("rabbitmq://localhost/test/");
            _inputQueueAddress = new Uri(_localHostUri, "input_queue");

            _sendObserver = new TestSendObserver(TestTimeout);
        }

        protected override IBus Bus
        {
            get { return _bus; }
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint
        {
            get { return _inputQueueSendEndpoint; }
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

        protected Uri LocalHostUri
        {
            get { return _localHostUri; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _localHostUri = value;
            }
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
            get { return _bus.Address; }
        }

        [TestFixtureSetUp]
        public void SetupInMemoryTestFixture()
        {
            _bus = CreateBus();

            Task<BusHandle> startTask = _bus.Start(TestCancellationToken);

            startTask.Wait(TestCancellationToken);

            _busHandle = startTask.Result;

            _busSendEndpoint = _bus.GetSendEndpoint(_bus.Address).Result;
            _busSendEndpoint.Connect(_sendObserver);

            _inputQueueSendEndpoint = _bus.GetSendEndpoint(_inputQueueAddress).Result;
            _inputQueueSendEndpoint.Connect(_sendObserver);
        }

        [TestFixtureTearDown]
        public void TearDownInMemoryTestFixture()
        {
            try
            {
                if (_busHandle != null)
                    _busHandle.Stop().Wait(TestTimeout);
            }
            catch (AggregateException ex)
            {
                _log.Error("LocalBus Stop Failed: ", ex);
                throw;
            }

            _bus = null;
        }

        protected virtual void ConfigureBus(IRabbitMqServiceBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
            {
                RabbitMqHostSettings host = x.Host(_localHostUri, h =>
                {
                });

                ConfigureBus(x);

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PrefetchCount = 16;
                    e.PurgeOnStartup();

                    ConfigureInputQueueEndpoint(e);
                });
            });
        }
    }
}