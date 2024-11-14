namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    public class TypeCastRetry_Specs :
        InMemoryTestFixture
    {
        #pragma warning disable NUnit1032
        Task<ConsumeContext<CreateCommand>> _received;
        #pragma warning restore NUnit1032

        [Test]
        public async Task Should_receive_the_message()
        {
            await Bus.Publish(new CreateCommand { Name = "Test" });

            await _received;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var sec5 = TimeSpan.FromSeconds(5);
            configurator.UseMessageRetry(x => x.Exponential(2, sec5, sec5, sec5));

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = HandledByConsumer<CreateCommand>(configurator);
        }


        class CreateCommand :
            Command
        {
            public CreateCommand()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; set; }
            public string Name { get; set; }
        }


        public interface ICommand
        {
        }


        public abstract class Command :
            ICommand
        {
        }
    }
}
