// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Serialization
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    [TestFixture]
    public class MisnamedProperty_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_carry_the_properties()
        {
            var message = "This is the real deal.";

            var request = new CommonRequest(message);

            IRequestClient<CommonRequest, CommonResponse> client = CreateRequestClient<CommonRequest, CommonResponse>();

            var response = await client.Request(request);

            Assert.That(response.Message, Is.EqualTo(message));
            Assert.That(response.Name, Is.EqualTo("I am lost!"));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<CommonRequestConsumer>();
        }


        public class CommonRequestConsumer : IConsumer<CommonRequest>
        {
            public Task Consume(ConsumeContext<CommonRequest> context)
            {
                CommonRequest newMessage = context.Message;

                var response = new CommonResponse(newMessage.Message, 5000, "I am lost!");

                context.Respond(response);

                return TaskUtil.Completed;
            }
        }
    }


    public class CommonRequest
    {
        public string Message { get; }

        public CommonRequest(string message)
        {
            Message = message; //Incorrect naming but it s ok
        }
    }


    public class CommonResponse
    {
        public string Message { get; }
        public int Cost { get; }
        public string Name { get; }

        public CommonResponse(string message, int cost, string name)
        {
            Message = message; //Correct
            Cost = cost; //Correct
            Name = name; //Incorrect! Namings not matching. This data will be lost during serialization
        }
    }
}