namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    [Category("Flaky")]
    public class When_mandatory_is_specified_and_no_binding_exists :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_throw_an_exception()
        {
            Assert.That(async () => await Bus.Publish(new NoBindingPlease(), context => context.Mandatory = true), Throws.TypeOf<MessageReturnedException>());
        }


        public class NoBindingPlease
        {
        }
    }
}
