using System.Threading;

namespace DistributedGrid.Activator
{
    using System;
    using Shared;
    using Shared.Messages;
    using MassTransit;

    public class CollectCompletedWork :
        Consumes<CompletedSimpleWorkItem>.All,
        IServiceInterface
    {
        private readonly IServiceBus _bus;
        private UnsubscribeAction _unsubscribeAction;

        public CollectCompletedWork(IServiceBus bus)
        {
            _bus = bus;

            Thread.Sleep(1000);

            for (int i = 0; i < 100; i++)
            {
                var g = Guid.NewGuid();
                Console.WriteLine("Publishing: " + g);
                _bus.Publish(new DoSimpleWorkItem(g));
            }
        }
        public void Consume(CompletedSimpleWorkItem message)
        {
            Console.WriteLine("Got Item");
        }

        public void Start()
        {
            _unsubscribeAction = _bus.Subscribe(this);
        }

        public void Stop()
        {
            var action = _unsubscribeAction;
            if (action != null)
            {
                action();
            }
        }
    }
}