namespace MassTransit.Transactions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.Extensions.Logging;
    using Util;


    public class TransactionOutboxEnlistment : IEnlistmentNotification
    {
        readonly ILogger<TransactionOutboxEnlistment> _logger;
        readonly List<Func<Task>> _pendingActions;

        public TransactionOutboxEnlistment(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<TransactionOutboxEnlistment>();
            _pendingActions = new List<Func<Task>>();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            _logger?.LogDebug("Prepare notification received");

            try
            {
                ExecutePendingActions();

                //If work finished correctly, reply prepared
                preparingEnlistment.Prepared();
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "MassTransit: Error executing pending actions");

                // We can't stop any messages that might have been already published, but we can stop the rest from publishing.
                // With a message broker, you should support idempotence, and so retry is a valid mechanism to handle this scenario

                preparingEnlistment.ForceRollback();
            }
        }

        public void Commit(Enlistment enlistment)
        {
            _logger?.LogDebug("Commit notification received");

            DiscardPendingActions();

            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            _logger?.LogDebug("Rollback notification received");

            DiscardPendingActions();

            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            _logger?.LogDebug("In doubt notification received");

            DiscardPendingActions();

            enlistment.Done();
        }

        public void Add(Func<Task> method)
        {
            lock (_pendingActions)
                _pendingActions.Add(method);
        }

        void ExecutePendingActions()
        {
            Func<Task>[] pendingActions;
            lock (_pendingActions)
                pendingActions = _pendingActions.ToArray();

            foreach (Func<Task> action in pendingActions)
                TaskUtil.Await(action);
        }

        void DiscardPendingActions()
        {
            lock (_pendingActions)
                _pendingActions.Clear();
        }
    }
}
