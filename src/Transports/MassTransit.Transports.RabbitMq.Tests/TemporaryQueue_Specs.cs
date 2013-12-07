// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;
    using BusConfigurators;
    using Magnum.Extensions;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_temporary_queue_is_specified :
        Given_a_service_bus_and_a_temporary_client
    {
        [Test]
        public void Should_be_able_to_request_response()
        {
            var responseReceived = new ManualResetEvent(false);

            LocalBus.GetEndpoint(RemoteUri).SendRequest(new Request(), LocalBus, x =>
                {
                    x.Handle<Response>((context, message) => responseReceived.Set());
                    x.SetTimeout(8.Seconds());
                });

            Assert.IsTrue(responseReceived.WaitOne(8.Seconds()), "No Response");
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(x => x.Handler<Request>((context, message) => context.Respond(new Response())));
        }


        class Request
        {
        }


        class Response
        {
        }
    }

    [TestFixture]
    public class When_a_temporary_queue_with_a_control_queue_is_created
    {
        [Test, Explicit]
        public void Should_remove_the_queues_and_exchanges_on_shutdown()
        {
            var uri = new Uri("rabbitmq://localhost/temporary_test_queue?temporary=true");
            using (IServiceBus bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom(uri);
                    x.UseRabbitMq();
                    x.UseControlBus();
                }))
            {
                Console.WriteLine("Using address: " + bus.Endpoint.Address.Uri);
                Console.WriteLine("Control bus address: " + bus.ControlBus.Endpoint.Address.Uri);

                Thread.Sleep(30000);
            }
        }
    }
}