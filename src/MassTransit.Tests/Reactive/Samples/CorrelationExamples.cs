namespace MassTransit.Tests.Reactive.Samples
{
    using System.Linq;
    using Messages;
    using NUnit.Framework;
    using MassTransit.Reactive;

    [TestFixture]
    public class CorrelationExamples
    {
        IServiceBus bus;

        [Test]
        public void GroupingByCorrelationId()
        {
            var o = from c in bus.AsObservable<PingMessage>()
                    group c by c.CorrelationId into g
                    select g;

        }
    }
}