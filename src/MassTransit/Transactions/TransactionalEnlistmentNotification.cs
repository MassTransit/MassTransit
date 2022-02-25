namespace MassTransit.Transactions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Transactions;
    using Context;
    using Logging;
    using Util;


    public class TransactionalEnlistmentNotification :
        IEnlistmentNotification
    {
        readonly List<Func<Task>> _pendingActions;

        public TransactionalEnlistmentNotification()
        {
            _pendingActions = new List<Func<Task>>();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            LogContext.Debug?.Log("Prepare notification received");

            try
            {
                ExecutePendingActions();

                //If work finished correctly, reply prepared
                preparingEnlistment.Prepared();
            }
            catch (Exception e)
            {
                LogContext.Error?.Log(e, "MassTransit: Error executing pending actions");

                // We can't stop any messages that might have been already published, but we can stop the rest from publishing.
                // With a message broker, you should support idempotence, and so retry is a valid mechanism to handle this scenario

                preparingEnlistment.ForceRollback(e);
            }
        }

        public void Commit(Enlistment enlistment)
        {
            LogContext.Debug?.Log("Commit notification received");

            DiscardPendingActions();

            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            LogContext.Debug?.Log("Rollback notification received");

            DiscardPendingActions();

            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            LogContext.Debug?.Log("In doubt notification received");

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
