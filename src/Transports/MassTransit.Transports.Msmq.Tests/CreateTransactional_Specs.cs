namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Messaging;
    using Magnum.TestFramework;

    [Scenario]
    public class When_creating_a_queue_with_transactional_enabled
    {
        MsmqEndpointAddress _address;
        IServiceBus _bus;

        [When]
        public void Creating_a_queue_with_transactional_enabled()
        {
            _address = new MsmqEndpointAddress(new Uri("msmq://localhost/created_transactional"));

            if(MessageQueue.Exists(_address.LocalName))
                MessageQueue.Delete(_address.LocalName);

            _bus = ServiceBusFactory.New(x =>
                {
                    x.UseMsmq();
                    x.ReceiveFrom(_address.Uri);

                    x.SetCreateMissingQueues(true);
                    x.SetCreateTransactionalQueues(true);
                });
        }

        [Finally]
        public void Finally()
        {
            if(_bus != null)
            {
                _bus.Dispose();
                _bus = null;
            }
        }

        [Then]
        public void Should_have_created_a_transactional_queue()
        {
            using (var queue = new MessageQueue(_address.LocalName))
            {
                queue.Transactional.ShouldBeTrue();
            }
        }
    }

    [Scenario]
    public class When_creating_a_queue_with_transactional_disable
    {
        MsmqEndpointAddress _address;
        IServiceBus _bus;

        [When]
        public void Creating_a_queue_with_transactional_enabled()
        {
            _address = new MsmqEndpointAddress(new Uri("msmq://localhost/created_nontransactional"));

            if(MessageQueue.Exists(_address.LocalName))
                MessageQueue.Delete(_address.LocalName);

            _bus = ServiceBusFactory.New(x =>
                {
                    x.UseMsmq();
                    x.ReceiveFrom(_address.Uri);

                    x.SetCreateMissingQueues(true);
                });
        }

        [Finally]
        public void Finally()
        {
            if(_bus != null)
            {
                _bus.Dispose();
                _bus = null;
            }
        }

        [Then]
        public void Should_have_created_a_non_transactional_queue()
        {
            using (var queue = new MessageQueue(_address.LocalName))
            {
                queue.Transactional.ShouldBeFalse();
            }
        }
    }
}
