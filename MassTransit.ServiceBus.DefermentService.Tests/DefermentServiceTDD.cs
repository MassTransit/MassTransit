namespace MassTransit.ServiceBus.DefermentService.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class DefermentServiceTDD
    {
        [Test]
        public void Doodle()
        {
            IMessage msg = null;
            IDefermentService d = new DefermentService();
            //when deferment completes we send back to the queue as if it had just come in.
            int defermentClaimTicket = d.Defer(msg, new TimeSpan(0, 0, 30));
        }
    }
}