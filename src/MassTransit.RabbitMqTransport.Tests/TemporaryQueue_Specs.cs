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
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_temporary_queue_is_specified :
        RabbitMqTestFixture
    {
        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<Request>(configurator, x => x.RespondAsync(new Response()));
        }


        class Request
        {
        }


        class Response
        {
        }


        [Test]
        public async Task Should_be_able_to_request_response()
        {
            Task<Response> responseTask = null;
            Task<Request<Request>> request = Bus.Request(InputQueueAddress, new Request(), x =>
            {
                responseTask = x.Handle<Response>();
                x.Timeout = TestTimeout;
            });

            await request;

            await responseTask;
        }
    }
}