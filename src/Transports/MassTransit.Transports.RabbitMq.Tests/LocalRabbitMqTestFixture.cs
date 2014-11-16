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
namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using Configuration;
    using EndpointConfigurators;
    using Logging;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class LocalRabbitMqTestFixture :
        LocalBusTestFixture
    {
        static readonly ILog _log = Logger.Get<LocalRabbitMqTestFixture>();
        IBusControl _localBus;
        Uri _localBusUri;
        Uri _localHostUri;

        public LocalRabbitMqTestFixture()
        {
            _localHostUri = new Uri("rabbitmq://localhost/test");
            _localBusUri = new Uri(_localHostUri, "input_queue");
        }

        protected override IBus LocalBus
        {
            get { return _localBus; }
        }

        protected Uri LocalBusUri
        {
            get { return _localBusUri; }
            set
            {
                if (LocalBus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _localBusUri = value;
            }
        }

        protected Uri LocalHostUri
        {
            get { return _localHostUri; }
            set
            {
                if (LocalBus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _localHostUri = value;
            }
        }

        [TestFixtureSetUp]
        public void SetupInMemoryTestFixture()
        {
            _localBus = CreateLocalBus();

            _localBus.Start(TestCancellationToken).Wait(TestTimeout);

//            ISendEndpoint sendEndpoint = _localBus.GetSendEndpoint(_localBusUri).Result;
//
//            _localSendEndpoint = new SendEndpointTestDecorator(sendEndpoint, TestTimeout);
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

            _localBus = null;
        }

        protected virtual void ConfigureLocalReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateLocalBus()
        {
            return ServiceBusFactory.New(x => x.RabbitMQ(), x =>
            {
                RabbitMqHostSettings host = x.Host(_localHostUri, h =>
                {
                });

                ConfigureLocalBus(x);

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    e.PrefetchCount(16);
                    e.Durable(false);
                    e.Exclusive();

                    e.Log(Console.Out, async context =>
                        string.Format("Received (input_queue): {0}", context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")));

                    ConfigureLocalReceiveEndpoint(e);
                });
            });
        }

        protected virtual void ConfigureLocalBus(IRabbitMqServiceBusFactoryConfigurator configurator)
        {
        }
    }
}