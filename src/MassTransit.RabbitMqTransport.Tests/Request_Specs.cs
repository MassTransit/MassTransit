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
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;


    public class When_sending_a_request_to_a_rabbitmq_endpoint :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_respond_properly()
        {
//            bool result = LocalBus.GetEndpoint(LocalBus.Endpoint.Address.Uri)
//                .SendRequest<PingMessage>(new PingImpl(), LocalBus, req =>
//                    {
//                        req.Handle<PongMessage>(x => { });
//                        req.SetTimeout(10.Seconds());
//                    });
//
//            result.ShouldBeTrue("No response was received.");
        }

        [Test]
        public void Should_respond_properly_using_async()
        {
//            Task<PongMessage> pongTask = null;
//            ITaskRequest<PingMessage> result = LocalBus.GetEndpoint(LocalBus.Endpoint.Address.Uri)
//                .SendRequestAsync<PingMessage>(LocalBus, new PingImpl(), req =>
//                    {
//                        pongTask = req.Handle<PongMessage>(x => { });
//                        req.SetTimeout(10.Seconds());
//                    });
//
//            result.Task.Wait(10.Seconds()).ShouldBeTrue("Request was not completed");
//
//            Assert.IsNotNull(pongTask, "Pong task should not be null");
//
//            pongTask.Wait(10.Seconds()).ShouldBeTrue("Pong was not completed");
        }

        [Test]
        public void Should_timeout_for_unhandled_request()
        {
            Assert.Throws<RequestTimeoutException>(() =>
            {
//                    LocalBus.GetEndpoint(LocalBus.Endpoint.Address.Uri)
//                        .SendRequest<PlinkMessage>(new PlinkMessageImpl(), LocalBus, req =>
//                            {
//                                req.Handle<PongMessage>(x => { });
//                                req.SetTimeout(8.Seconds());
//                            });
            });
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<PingHandler>();
        }


        class PingHandler
            : IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return context.RespondAsync<PongMessage>(new PongImpl());
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
    }
}