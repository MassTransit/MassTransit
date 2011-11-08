namespace OpenAllNight.Testers
{
    using System;
    using MassTransit;
    using MassTransit.Services.Subscriptions.Messages;

    public class SubscriptionServiceTester
    {
        private readonly IEndpoint _subscriptionEndpoint;
        private IServiceBus _bus;
        private Counter _counter;
        Random _rand = new Random();
        private static UnsubscribeAction _unsubscribeToken = () => false;
        MessageCounter _handler = new MessageCounter();

        public SubscriptionServiceTester(IEndpoint subscriptionEndpoint, IServiceBus bus, Counter counter)
        {
            _subscriptionEndpoint = subscriptionEndpoint;
            _bus = bus;
            _counter = counter;
        }

        public void Test()
        {
            Guid ticket = Guid.NewGuid();
            _subscriptionEndpoint.Send(new AddSubscriptionClient(ticket, _bus.Endpoint.Address.Uri, _bus.Endpoint.Address.Uri));
            _counter.IncrementMessagesSent();

            if (_rand.Next(0, 10) == 0)
            {
                _unsubscribeToken();
                _unsubscribeToken = _bus.SubscribeInstance(_handler);
                _counter.Subscribed = true;
            }
            else if (_rand.Next(0, 10) == 0)
            {
                _unsubscribeToken();
                _unsubscribeToken = () => false;
                _counter.Subscribed = false;
            }

            if (_rand.Next(0, 10) < 4)
            {
                _bus.Publish(new SimpleMessage());
                _counter.IncrementPublishCount();
            }
			_subscriptionEndpoint.Send(new RemoveSubscriptionClient(ticket, _bus.Endpoint.Address.Uri, _bus.Endpoint.Address.Uri));
        }

        public void Results()
        {
            Console.WriteLine("Messages Sent: {0}", _counter.MessagesSent);
            Console.WriteLine("Messages Received: {0}", _counter.MessagesReceived);
        }
    }
}