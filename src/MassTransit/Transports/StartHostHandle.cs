// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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
        readonly IBusHostControl _host;
        readonly IAgent[] _readyAgents;

        public StartHostHandle(IBusHostControl host, HostReceiveEndpointHandle[] handles, params IAgent[] readyAgents)
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