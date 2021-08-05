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

            Assert.IsTrue(_firstCalled);
            Assert.IsTrue(_firstRequested.HasValue);
            Assert.IsFalse(_firstRequested.Value);
            Assert.IsFalse(_secondCalled);
            Assert.IsFalse(_secondRequested.HasValue);
        }

        Task _succeeded;
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
