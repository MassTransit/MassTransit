using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.SignalR.Sample.HostedServices
{
    public class BusService :
    IHostedService
    {
        private readonly IBusControl _busControl;

        public BusService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _busControl.StopAsync(cancellationToken);
        }
    }
}
