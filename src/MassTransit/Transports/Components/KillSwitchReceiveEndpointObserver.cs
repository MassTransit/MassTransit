namespace MassTransit.Transports.Components
{
    using System.Threading.Tasks;


    public class KillSwitchReceiveEndpointObserver :
        IReceiveEndpointObserver
    {
        readonly KillSwitchOptions _options;

        public KillSwitchReceiveEndpointObserver(KillSwitchOptions options)
        {
            _options = options;
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            ready.ReceiveEndpoint.ConnectConsumeObserver(new KillSwitch(_options, ready.ReceiveEndpoint));

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
