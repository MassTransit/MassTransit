namespace Grid.Distributor.Activator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using MassTransit;
    using MassTransit.Logging;
    using Shared.Messages;
    using Topshelf;


    public class ActivatorService :
        Consumes<CompletedSimpleWorkItem>.All,
        ServiceControl
    {
        readonly ILog _log = Logger.Get(typeof(ActivatorService));
        readonly List<int> _values = new List<int>();
        int _count;
        IServiceBus _dataBus;
        HostControl _hostControl;
        int _received;
        int _sent;

        public void Consume(CompletedSimpleWorkItem message)
        {
            Interlocked.Increment(ref _received);

            int messageMs = DateTime.UtcNow.Subtract(message.RequestCreatedAt).Milliseconds;

            int max;
            int min;
            double average;
            int count;
            lock (_values)
            {
                _values.Add(messageMs);
                min = _values.Min();
                max = _values.Max();
                average = _values.Average();
                count = _values.Count;
            }

            _log.InfoFormat(
                "Received: {0}/{3} - {1} [{2}ms]\nStats:\tMin: {4:0000.0}ms\tMax: {5:0000.0}ms\tAvg: {6:0000.0}ms",
                _received, message.CorrelationId, messageMs, count, min, max, average);

            if (count == _count)
                _hostControl.Stop();
        }

        public bool Start(HostControl hostControl)
        {
            _hostControl = hostControl;
            _dataBus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("rabbitmq://localhost/mt_grid_activator?prefetch=4");

                    x.UseRabbitMq();
                    x.SetConcurrentConsumerLimit(4);

                    x.Subscribe(s => s.Instance(this).Transient());

                    x.Distributor(d => d.Handler<DoSimpleWorkItem>());
                });

            _count = 1000;
            for (int i = 0; i < _count; i++)
            {
                Guid correlationId = NewId.NextGuid();
                _dataBus.Endpoint.Send<DoSimpleWorkItem>(new DoSimpleWorkItemImpl(correlationId));

                Interlocked.Increment(ref _sent);
            }

            return true;
        }


        public bool Stop(HostControl hostControl)
        {
            _dataBus.Dispose();

            return true;
        }


        public class DoSimpleWorkItemImpl :
            DoSimpleWorkItem
        {
            public DoSimpleWorkItemImpl(Guid correlationId)
            {
                CorrelationId = correlationId;
                CreatedAt = DateTime.UtcNow;
            }

            public Guid CorrelationId { get; private set; }
            public DateTime CreatedAt { get; private set; }
        }
    }
}