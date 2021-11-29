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

            IRequestClient<CommonRequest> client = CreateRequestClient<CommonRequest>();

            Response<CommonResponse> response = await client.GetResponse<CommonResponse>(request);

            Assert.That(response.Message.Message, Is.EqualTo(message));
            Assert.That(response.Message.Name, Is.EqualTo("I am lost!"));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<CommonRequestConsumer>();
        }


        public class CommonRequestConsumer : IConsumer<CommonRequest>
        {
            public Task Consume(ConsumeContext<CommonRequest> context)
            {
                var newMessage = context.Message;

                var response = new CommonResponse(newMessage.Message, 5000, "I am lost!");

                context.Respond(response);

                return Task.CompletedTask;
            }
        }
    }


    public class CommonRequest
    {
        public CommonRequest(string message)
        {
            Message = message; //Incorrect naming but it s ok
        }

        public string Message { get; }
    }


    public class CommonResponse
    {
        public CommonResponse(string message, int cost, string name)
        {
            Message = message; //Correct
            Cost = cost; //Correct
            Name = name; //Incorrect! Namings not matching. This data will be lost during serialization
        }

        public string Message { get; }
        public int Cost { get; }
        public string Name { get; }
    }
}
