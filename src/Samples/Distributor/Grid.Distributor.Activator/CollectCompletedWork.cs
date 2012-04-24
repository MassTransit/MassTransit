namespace Grid.Distributor.Activator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using MassTransit;
    using Shared;
    using Shared.Messages;
    using log4net;

    public class CollectCompletedWork :
        Consumes<CompletedSimpleWorkItem>.All,
        IServiceInterface
    {
        readonly ILog _log = LogManager.GetLogger(typeof(CollectCompletedWork));
        readonly List<int> _values = new List<int>();
        int _received;
        int _sent;
        UnsubscribeAction _unsubscribeAction;

        public CollectCompletedWork()
        {
            DataBus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom(ConfigurationManager.AppSettings["SourceQueue"]);
                    x.SetPurgeOnStartup(true);

                    x.UseMsmq();
                    x.UseMulticastSubscriptionClient();
                    x.UseControlBus();

                    x.SetConcurrentConsumerLimit(4);
                    
                    x.Distributor(d => d.Handler<DoSimpleWorkItem>());
                });

            ControlBus = DataBus.ControlBus;
        }

        public IServiceBus ControlBus { get; set; }
        public IServiceBus DataBus { get; set; }

        public void Consume(CompletedSimpleWorkItem message)
        {
            Interlocked.Increment(ref _received);

            int messageMs = DateTime.UtcNow.Subtract(message.RequestCreatedAt).Milliseconds;

            int max;
            int min;
            double average;
            lock (_values)
            {
                _values.Add(messageMs);
                min = _values.Min();
                max = _values.Max();
                average = _values.Average();
            }

            _log.InfoFormat("Received: {0} - {1} [{2}ms]", _received, message.CorrelationId, messageMs);
            _log.InfoFormat("Stats\n\tMin: {0:0000.0}ms\n\tMax: {1:0000.0}ms\n\tAvg: {2:0000.0}ms", min, max, average);
        }

        public void Start()
        {
            _unsubscribeAction = DataBus.SubscribeInstance(this);

            Thread.Sleep(5000);

            DataBus.OutboundPipeline.View(Console.WriteLine);

            Thread.Sleep(5000);
            Thread.Sleep(5000);
            Thread.Sleep(5000);

            for (int i = 0; i < 100; i++)
            {
                Guid g = Guid.NewGuid();
                _log.InfoFormat("Publishing: {0}", g);
                DataBus.Publish(new DoSimpleWorkItem(g));

                Interlocked.Increment(ref _sent);
            }
        }

        public void Stop()
        {
            UnsubscribeAction action = _unsubscribeAction;
            if (action != null)
            {
                action();
            }
        }
    }
}