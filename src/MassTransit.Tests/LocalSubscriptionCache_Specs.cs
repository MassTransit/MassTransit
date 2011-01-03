// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Tests
{
    using System;
    using Configuration;
    using MassTransit.Serialization;
    using MassTransit.Services.Subscriptions;
    using MassTransit.Transports;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TestConsumers;

    [TestFixture]
    public class When_a_handler_subscription_is_added :
        Specification
    {
        private ServiceBus _serviceBus;
        private IEndpoint _mockEndpoint;
        private readonly Uri queueUri = new Uri("loopback://localhost/test");
        private Subscription _subscription;
        private IObjectBuilder _builder;
        private IEndpointResolver _endpointResolver;

    	protected override void Before_each()
        {
            _builder = MockRepository.GenerateMock<IObjectBuilder>();
        	_builder.Stub(x => x.GetInstance<XmlMessageSerializer>()).Return(new XmlMessageSerializer());
			_endpointResolver = EndpointResolverConfigurator.New(x =>
			{
				x.SetObjectBuilder(_builder);
				x.RegisterTransport<LoopbackTransportFactory>();
			});

            _mockEndpoint = _endpointResolver.GetEndpoint(queueUri);

            _subscription = new Subscription(typeof (PingMessage), queueUri);
        	_serviceBus = new ServiceBus(_mockEndpoint, _builder, _endpointResolver);
        }

        protected override void After_each()
        {
            _serviceBus = null;
            _mockEndpoint = null;
        }


        [Test]
        public void A_subscription_should_be_added_for_a_consumer()
        {
            using (Record())
            {
//                _mockSubscriptionCache.Add(_subscription);
            }

            using (Playback())
            {
                var consumer = new TestMessageConsumer<PingMessage>();

                _serviceBus.Subscribe(consumer);
            }
        }


        [Test]
        public void A_subscription_should_be_added_for_a_selective_consumer()
        {
            using (Record())
            {
  //              _mockSubscriptionCache.Add(_subscription);
            }

            using (Playback())
            {
                var consumer = new TestSelectiveConsumer<PingMessage>();

                _serviceBus.Subscribe(consumer);
            }
        }

        [Test]
        public void The_bus_should_add_a_subscription_to_the_subscription_cache()
        {
            using (Record())
            {
    //            _mockSubscriptionCache.Add(_subscription);
            }

            using (Playback())
            {
                _serviceBus.Subscribe<PingMessage>(delegate { });
            }
        }
    }
}