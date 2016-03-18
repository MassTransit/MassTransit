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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading;
    using Logging;
    using MassTransit.Testing;
    using MassTransit.Testing.TestDecorators;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using TestFramework;
    using Transports;


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
        Uri _hostAddress;
        IMessageNameFormatter _nameFormatter;
        BusHandle _busHandle;

        public RabbitMqTestFixture()
        {
            _hostAddress = new Uri("rabbitmq://[::1]/test/");
            _inputQueueAddress = new Uri(_hostAddress, "input_queue");

            _sendObserver = new TestSendObserver(TestTimeout);
        }

        protected override IBus Bus => _bus;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => _inputQueueSendEndpoint;

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

        protected Uri HostAddress
        {
            get { return _hostAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _hostAddress = value;
            }
        }

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => _busSendEndpoint;

        protected ISentMessageList Sent => _sendObserver.Messages;

        protected Uri BusAddress => _bus.Address;

        [TestFixtureSetUp]
        public void SetupInMemoryTestFixture()
        {
            _bus = CreateBus();

            _busHandle = _bus.Start();
            try
            {
                _busSendEndpoint = Await(() => _bus.GetSendEndpoint(_bus.Address));
                _busSendEndpoint.ConnectSendObserver(_sendObserver);

                _inputQueueSendEndpoint = Await(() => _bus.GetSendEndpoint(_inputQueueAddress));
                _inputQueueSendEndpoint.ConnectSendObserver(_sendObserver);
            }
            catch (Exception)
            {
                try
                {
                    using (var tokenSource = new CancellationTokenSource(TestTimeout))
                    {
                        _busHandle?.Stop(tokenSource.Token);
                    }
                }
                finally
                {
                    _busHandle = null;
                    _bus = null;
                }

                throw;
            }
        }

        [TestFixtureTearDown]
        public void TearDownInMemoryTestFixture()
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(TestTimeout))
                {
                    _bus.Stop(tokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Bus Stop Failed", ex);
            }
            finally
            {
                _busHandle = null;
                _bus = null;
            }
        }

        protected virtual void ConfigureBus(IRabbitMqBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
        }

        protected virtual void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
        }

        protected virtual void OnCleanupVirtualHost(IModel model)
        {
        }

        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
            {
                ConfigureBus(x);

                IRabbitMqHost host = x.Host(_hostAddress, h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                CleanUpVirtualHost(host);

                ConfigureBusHost(x, host);

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PrefetchCount = 16;
                    e.PurgeOnStartup = true;

                    ConfigureInputQueueEndpoint(e);
                });
            });
        }

        protected IMessageNameFormatter NameFormatter => _nameFormatter;

        void CleanUpVirtualHost(IRabbitMqHost host)
        {
            try
            {
                _nameFormatter = new RabbitMqMessageNameFormatter();

                ConnectionFactory connectionFactory = host.Settings.GetConnectionFactory();
                using (IConnection connection = connectionFactory.CreateConnection())
                using (IModel model = connection.CreateModel())
                {
                    model.ExchangeDelete("input_queue");
                    model.QueueDelete("input_queue");

                    model.ExchangeDelete("input_queue_skipped");
                    model.QueueDelete("input_queue_skipped");

                    model.ExchangeDelete("input_queue_error");
                    model.QueueDelete("input_queue_error");

                    model.ExchangeDelete("input_queue_delay");
                    model.QueueDelete("input_queue_delay");

                    OnCleanupVirtualHost(model);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}