namespace MassTransit.Tests.Transforms
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Settings_a_property_on_a_message :
        InMemoryTestFixture
    {
        Task<ConsumeContext<A>> _received;

        [Test]
        public async void Should_have_the_message_property()
        {
            await InputQueueSendEndpoint.Send(new A {First = "Hello"});

            var result = await _received;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe("World");

        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInputQueueEndpoint(configurator);

            configurator.UseTransform<A>(t =>
            {
                // Replace modifies the original message, versus Set which leaves the original message unmodified
                t.Replace(x => x.Second, "World");
            });

            _received = Handler<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }
}
