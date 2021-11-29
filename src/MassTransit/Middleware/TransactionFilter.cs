namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Transactions;
    using Context;


    public class TransactionFilter<T> :
        IFilter<T>
        where T : class, PipeContext
    {
        readonly TransactionOptions _options;

        public TransactionFilter(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, TimeSpan timeout = default)
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(30);

            _options = new TransactionOptions
            {
                IsolationLevel = isolationLevel,
                Timeout = timeout
            };
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var step = context.CreateFilterScope("transaction");
            step.Add("isolationLevel", _options.IsolationLevel.ToString());
            step.Add("timeout", _options.Timeout);
        }

        [DebuggerNonUserCode]
        public async Task Send(T context, IPipe<T> next)
        {
            SystemTransactionContext systemTransactionContext = null;
            context.GetOrAddPayload<TransactionContext>(() =>
            {
                systemTransactionContext = new SystemTransactionContext(_options);

                return systemTransactionContext;
            });

            try
            {
                await next.Send(context).ConfigureAwait(false);

                if (systemTransactionContext != null)
                    await systemTransactionContext.Commit().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                systemTransactionContext?.Rollback(ex);

                throw;
            }
            finally
            {
                systemTransactionContext?.Dispose();
            }
        }
    }
}
