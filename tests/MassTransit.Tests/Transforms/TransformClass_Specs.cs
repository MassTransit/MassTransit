namespace MassTransit.Tests.Transforms
{
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_a_class_to_define_a_transform :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            await InputQueueSendEndpoint.Send(new A { First = "Hello" });

            ConsumeContext<A> result = await _received;

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.First, Is.EqualTo("First"));
                Assert.That(result.Message.Second, Is.EqualTo("Second"));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _received;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseTransform<A>(x => x.Get<FullTransform>());

            _received = Handled<A>(configurator);
        }


        class FullTransform :
            ConsumeTransformSpecification<A>
        {
            public FullTransform()
            {
                Set(x => x.First, context => "First");
                Set(x => x.Second, context => "Second");
            }
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }
}
