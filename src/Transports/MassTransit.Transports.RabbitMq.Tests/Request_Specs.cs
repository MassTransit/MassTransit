// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using BusConfigurators;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;

    [Scenario]
    public class When_sending_a_request_to_a_rabbitmq_endpoint :
        Given_a_rabbitmq_bus
    {
        protected override void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
        {
            base.ConfigureServiceBus(uri, configurator);

            configurator.Subscribe(x => { x.Consumer<PingHandler>(); });
        }

        class PingHandler
            : Consumes<IConsumeContext<PingMessage>>.All
        {
            public void Consume(IConsumeContext<PingMessage> context)
            {
                context.Respond<PongMessage>(new PongImpl());
            }
        }

        public interface PingMessage
        {
        }

        public interface PlinkMessage
        {
        }

        public interface PongMessage
        {
        }

        class PingImpl :
            PingMessage
        {
        }

        class PlinkMessageImpl : 
            PlinkMessage
        {
        }

        class PongImpl :
            PongMessage
        {
        }

        [Test]
        public void Should_respond_properly()
        {
            bool result = LocalBus.GetEndpoint(LocalBus.Endpoint.Address.Uri)
                .SendRequest<PingMessage>(new PingImpl(), LocalBus, req =>
                    {
                        req.Handle<PongMessage>(x => { });
                        req.SetTimeout(10.Seconds());
                    });

            result.ShouldBeTrue("No response was received.");
        }

        [Test]
        public void Should_respond_after_changing_qos()
        {
            LocalBus.SetPrefetchCount(197);

            bool result = LocalBus.GetEndpoint(LocalBus.Endpoint.Address.Uri)
                .SendRequest<PingMessage>(new PingImpl(), LocalBus, req =>
                {
                    req.Handle<PongMessage>(x => { });
                    req.SetTimeout(10.Seconds());
                });

            result.ShouldBeTrue("No response was received.");            
        }

        [Test]
        public void Should_timeout_for_unhandled_request()
        {
            Assert.Throws<RequestTimeoutException>(() =>
                {
                    LocalBus.GetEndpoint(LocalBus.Endpoint.Address.Uri)
                        .SendRequest<PlinkMessage>(new PlinkMessageImpl(), LocalBus, req =>
                            {
                                req.Handle<PongMessage>(x => { });
                                req.SetTimeout(8.Seconds());
                            });
                });
        }
    }
}