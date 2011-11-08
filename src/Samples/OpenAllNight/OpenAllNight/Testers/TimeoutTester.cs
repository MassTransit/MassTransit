namespace OpenAllNight.Testers
{
    using System;
    using Magnum.Extensions;
    using MassTransit;
    using MassTransit.Services.Timeout.Messages;

    public class TimeoutTester :
        Consumes<TimeoutExpired>.For<Guid>

    {
        private IServiceBus _bus;
        private Guid _ticket;

        public TimeoutTester(IServiceBus bus)
        {
            _bus = bus;
            _bus.SubscribeInstance(this);
            _ticket = Guid.NewGuid();
        }

        public void Test()
        {
            _bus.Publish(new ScheduleTimeout(_ticket, 1.Seconds()));
        }

        public Guid CorrelationId
        {
            get { return _ticket; }
        }

        public void Consume(TimeoutExpired message)
        {
            Console.WriteLine("Timeout Expired: Whoot!");
        }
    }
}