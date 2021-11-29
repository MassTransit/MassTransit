namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class PendingFaultCollection
    {
        readonly IList<IPendingFault> _pendingFaults;

        public PendingFaultCollection()
        {
            _pendingFaults = new List<IPendingFault>();
        }

        public void Add<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
            where T : class
        {
            var pendingFault = new PendingFault<T>(context, elapsed, consumerType, exception);

            lock (_pendingFaults)
                _pendingFaults.Add(pendingFault);
        }

        public async Task Notify(ConsumeContext consumeContext)
        {
            IPendingFault[] pendingFaults;
            do
            {
                lock (_pendingFaults)
                {
                    if (_pendingFaults.Count == 0)
                        return;

                    pendingFaults = _pendingFaults.ToArray();

                    _pendingFaults.Clear();
                }

                await Task.WhenAll(pendingFaults.Select(x => x.Notify(consumeContext))).ConfigureAwait(false);
            }
            while (pendingFaults.Length > 0);
        }


        interface IPendingFault
        {
            Task Notify(ConsumeContext context);
        }


        class PendingFault<T> :
            IPendingFault
            where T : class
        {
            readonly string _consumerType;
            readonly ConsumeContext<T> _context;
            readonly TimeSpan _elapsed;
            readonly Exception _exception;

            public PendingFault(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
            {
                _context = context;
                _elapsed = elapsed;
                _consumerType = consumerType;
                _exception = exception;
            }

            public Task Notify(ConsumeContext context)
            {
                return context.NotifyFaulted(_context, _elapsed, _consumerType, _exception);
            }
        }
    }
}
