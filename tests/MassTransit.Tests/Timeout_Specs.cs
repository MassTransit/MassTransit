namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Timeout_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_throw_on_timeout()
        {
            Task<ConsumeContext<Fault<PingMessage>>> faulted = await ConnectPublishHandler<Fault<PingMessage>>();

            await InputQueueSendEndpoint.Send(new PingMessage());
            await Task.WhenAny(_succeeded, faulted);

            Assert.Multiple(() =>
            {
                Assert.That(_firstCalled, Is.True);
                Assert.That(_firstRequested.HasValue, Is.True);
                Assert.That(_firstRequested.Value, Is.False);
                Assert.That(_secondCalled, Is.False);
                Assert.That(_secondRequested.HasValue, Is.False);
            });
        }

        #pragma warning disable NUnit1032
        Task _succeeded;
        #pragma warning restore NUnit1032
        bool _firstCalled;
        bool? _firstRequested;
        bool _secondCalled;
        bool? _secondRequested;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseTimeout(c => c.Timeout = TimeSpan.FromMilliseconds(100));

            _succeeded = Handler<PingMessage>(configurator, async context =>
            {
                _firstCalled = true;
                _firstRequested = context.CancellationToken.IsCancellationRequested;

                await Task.Delay(1000, context.CancellationToken);

                _secondCalled = true;
                _secondRequested = context.CancellationToken.IsCancellationRequested;
            });
        }
    }
}
