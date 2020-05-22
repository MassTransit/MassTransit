namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using GreenPipes.Agents;


    public class StartHostHandle :
        HostHandle
    {
        readonly HostReceiveEndpointHandle[] _handles;
        readonly IHost _host;
        readonly IAgent[] _readyAgents;

        public StartHostHandle(IHost host, HostReceiveEndpointHandle[] handles, params IAgent[] readyAgents)
        {
            _host = host;
            _handles = handles;
            _readyAgents = readyAgents;
        }

        Task<HostReady> HostHandle.Ready
        {
            get { return ReadyOrNot(_handles.Select(x => x.Ready)); }
        }

        Task HostHandle.Stop(CancellationToken cancellationToken)
        {
            return _host.Stop("Stopping Host", cancellationToken);
        }

        async Task<HostReady> ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
        {
            Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();
            foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                await ready.ConfigureAwait(false);

            foreach (var agent in _readyAgents)
                await agent.Ready.ConfigureAwait(false);

            ReceiveEndpointReady[] endpointsReady = await Task.WhenAll(readyTasks).ConfigureAwait(false);

            return new HostReadyEvent(_host.Address, endpointsReady);
        }
    }
}
