using MassTransit.SignalR.Contracts;
using Microsoft.AspNetCore.SignalR.Protocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit.SignalR.Utils;
using Microsoft.AspNetCore.SignalR;
using GreenPipes;
using System.Threading;
using System.Collections.Concurrent;
using System.Transactions;
using MassTransit.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MassTransit.SignalR.SampleConsole
{
    static class Program
    {
        internal static async Task Main(string[] args)
        {
            //IReadOnlyList<IHubProtocol> protocols = new IHubProtocol[] { new JsonHubProtocol() };
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            // Important! The bus must be started before using it!
            await busControl.StartAsync();

            do
            {
                Console.WriteLine("Enter message (or quit to exit)");
                Console.Write("> ");
                string value = Console.ReadLine();

                if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                var test = new TransactionOutbox(busControl, new NullLoggerFactory());

                await test.Publish<MyMessage>(new
                {
                    Name = "John"
                });
            }
            while (true);

            await busControl.StopAsync();
        }
    }

    public class MyMessage
    {
        public string Name { get; set; }
    }

    public class TransactionOutbox : IPublishEndpoint
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPublishEndpoint _publishEndpoint;
        readonly ConcurrentDictionary<Transaction, TransactionOutboxEnlistment> _pendingActions;

        public TransactionOutbox(IPublishEndpoint publishEndpoint, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _publishEndpoint = publishEndpoint;
            _pendingActions = new ConcurrentDictionary<Transaction, TransactionOutboxEnlistment>();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default) where T : class
        {
            if (Transaction.Current == null)
                return _publishEndpoint.Publish(values, cancellationToken);

            var pendingActions = _pendingActions.GetOrAdd(Transaction.Current, new TransactionOutboxEnlistment(_loggerFactory));
            pendingActions.Add(() => _publishEndpoint.Publish(values, cancellationToken));

            return Task.CompletedTask;
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class
        {
            throw new NotImplementedException();
        }
    }

    public class TransactionOutboxEnlistment : IEnlistmentNotification
    {
        private readonly ILogger<TransactionOutboxEnlistment> _logger;
        readonly List<Func<Task>> _pendingActions;

        public TransactionOutboxEnlistment(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TransactionOutboxEnlistment>();
            _pendingActions = new List<Func<Task>>();
        }

        public void Add(Func<Task> method)
        {
            lock (_pendingActions)
                _pendingActions.Add(method);
        }

        public void ExecutePendingActions()
        {
            Func<Task>[] pendingActions;
            lock (_pendingActions)
                pendingActions = _pendingActions.ToArray();

            foreach (var action in pendingActions)
            {
                TaskUtil.Await(action);
            }
        }

        public void DiscardPendingActions()
        {
            lock (_pendingActions)
                _pendingActions.Clear();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            _logger.LogDebug("Prepare notification received");

            try
            {
                ExecutePendingActions();

                //If work finished correctly, reply prepared
                preparingEnlistment.Prepared();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "MASSTRANSIT: Error executing pending actions");
                // We can't stop any messages that might have been already published, but we can stop the rest from publishing
                // Realizing that with message brokers, idempotence is important, because you can receive duplicate messages
                preparingEnlistment.ForceRollback();
            }
        }

        public void Commit(Enlistment enlistment)
        {
            _logger.LogDebug("Commit notification received");

            DiscardPendingActions();

            //Declare done on the enlistment
            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            _logger.LogDebug("Rollback notification received");

            DiscardPendingActions();

            //Declare done on the enlistment
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            _logger.LogDebug("In doubt notification received");

            DiscardPendingActions();

            //Declare done on the enlistment
            enlistment.Done();
        }
    }
}
