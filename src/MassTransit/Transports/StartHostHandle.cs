namespace MassTransit.Transports
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;


    public class StartHostHandle :
        HostHandle
    {
        readonly HostReceiveEndpointHandle[] _handles;
        readonly BaseHost _host;
        readonly HostRiderHandle[] _riderHandles;

        public StartHostHandle(BaseHost host, HostReceiveEndpointHandle[] handles, HostRiderHandle[] riderHandles)
        {
            _host = host;
            _handles = handles;
            _riderHandles = riderHandles;
        }

        Task<HostReady> HostHandle.Ready
        {
            get { return ReadyOrNot(_handles.Select(x => x.Ready).ToArray(), _riderHandles.Select(x => x.Ready).ToArray()); }
        }

        Task HostHandle.Stop(CancellationToken cancellationToken)
        {
            return _host.Stop(cancellationToken);
        }

        async Task<HostReady> ReadyOrNot(Task<ReceiveEndpointReady>[] endpoints, Task<RiderReady>[] riders)
        {
            ReceiveEndpointReady[] endpointsReady = await EndpointsReady(endpoints).ConfigureAwait(false);

            RiderReady[] ridersReady = await RidersReady(riders).ConfigureAwait(false);

            return new HostReadyEvent(_host.Address, endpointsReady, ridersReady);
        }

        static async Task<ReceiveEndpointReady[]> EndpointsReady(Task<ReceiveEndpointReady>[] endpoints)
        {
            foreach (Task<ReceiveEndpointReady> ready in endpoints)
                await ready.ConfigureAwait(false);

            return await Task.WhenAll(endpoints).ConfigureAwait(false);
        }

        static async Task<RiderReady[]> RidersReady(Task<RiderReady>[] riders)
        {
            foreach (Task<RiderReady> ready in riders)
                await ready.ConfigureAwait(false);

            return await Task.WhenAll(riders).ConfigureAwait(false);
        }
    }
}
