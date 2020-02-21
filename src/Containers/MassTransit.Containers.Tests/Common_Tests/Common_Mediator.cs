namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using TestFramework;


    public abstract class Common_Mediator :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_dispatch_to_the_consumer()
        {
            const string name = "Joe";

            await Mediator.Send(new SimpleMessageClass(name));

            SimplerConsumer lastConsumer = await SimplerConsumer.LastConsumer.OrCanceled(TestCancellationToken);
            lastConsumer.ShouldNotBe(null);

            await lastConsumer.Last.OrCanceled(TestCancellationToken);
        }

        protected abstract IMediator Mediator { get; }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
        {
            configurator.AddConsumer<SimplerConsumer>();

            configurator.AddMediator();
        }
    }
}
