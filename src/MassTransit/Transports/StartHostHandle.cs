namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes.Agents;
    using Riders;


    public class StartHostHandle :
        HostHandle
    {
        readonly IAgent[] _agents;
        readonly HostReceiveEndpointHandle[] _handles;
        readonly BaseHost _host;
        readonly HostRiderHandle[] _riderHandles;

        public StartHostHandle(BaseHost host, HostReceiveEndpointHandle[] handles, HostRiderHandle[] riderHandles, params IAgent[] agents)
        {
            _host = host;
            _handles = handles;
            _riderHandles = riderHandles;
            _agents = agents;
        }

        Task<HostReady> HostHandle.Ready
        {
            get { return ReadyOrNot(_handles.Select(x => x.Ready), _riderHandles.Select(x => x.Ready)); }
        }

        Task HostHandle.Stop(CancellationToken cancellationToken)
        {
            return _host.Stop(cancellationToken);
        }

        async Task<HostReady> ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints, IEnumerable<Task<RiderReady>> riders)
        {
            ReceiveEndpointReady[] endpointsReady = await EndpointsReady(endpoints).ConfigureAwait(false);

            RiderReady[] ridersReady = await RidersReady(riders).ConfigureAwait(false);

            foreach (var agent in _agents)
                await agent.Ready.ConfigureAwait(false);

            return new HostReadyEvent(_host.Address, endpointsReady, ridersReady);
        }

        static async Task<ReceiveEndpointReady[]> EndpointsReady(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
        {
            Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();
            foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                await ready.ConfigureAwait(false);

            return await Task.WhenAll(readyTasks).ConfigureAwait(false);
        }

        static async Task<RiderReady[]> RidersReady(IEnumerable<Task<RiderReady>> riders)
        {
            Task<RiderReady>[] readyTasks = riders as Task<RiderReady>[] ?? riders.ToArray();
            foreach (Task<RiderReady> ready in readyTasks)
                await ready.ConfigureAwait(false);

            return await Task.WhenAll(readyTasks).ConfigureAwait(false);
        }
    }
}
