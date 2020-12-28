namespace MassTransit.Transports.Components
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class KillSwitchReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        readonly IDictionary<Uri, KillSwitch> _killSwitches;
        readonly KillSwitchOptions _options;

        public KillSwitchReceiveEndpointObserver(KillSwitchOptions options)
        {
            _options = options;

            _killSwitches = new Dictionary<Uri, KillSwitch>();
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            lock (_killSwitches)
            {
                if (!_killSwitches.TryGetValue(ready.InputAddress, out var killSwitch))
                {
                    killSwitch = new KillSwitch(_options, ready.ReceiveEndpoint);

                    _killSwitches.Add(ready.InputAddress, killSwitch);

                    ready.ReceiveEndpoint.ConnectConsumeObserver(killSwitch);
                }
            }

            return Task.CompletedTask;
        }

        public Task Stopping(ReceiveEndpointStopping stopping)
        {
            return Task.CompletedTask;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return Task.CompletedTask;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            return Task.CompletedTask;
        }
    }
}
