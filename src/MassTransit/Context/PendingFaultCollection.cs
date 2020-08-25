namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    class PendingFaultCollection
    {
        readonly IList<IPendingFault> _pendingFaults;

        public PendingFaultCollection()
        {
            _pendingFaults = new List<IPendingFault>();
        }

        public void Add(IPendingFault pendingFault)
        {
            if (pendingFault == null)
                throw new ArgumentNullException(nameof(pendingFault));

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
    }
}
