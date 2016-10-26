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


    public abstract class BaseHostHandle :
        HostHandle
    {
        readonly HostReceiveEndpointHandle[] _handles;
        readonly IHost _host;

        protected BaseHostHandle(IHost host, HostReceiveEndpointHandle[] handles)
        {
            _host = host;
            _handles = handles;
        }

        public Task<HostReady> Ready
        {
            get { return ReadyOrNot(_handles.Select(x => x.Ready)); }
        }

        public virtual async Task Stop(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_handles.Select(x => x.StopAsync(cancellationToken))).ConfigureAwait(false);
        }

        async Task<HostReady> ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
        {
            Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();
            foreach (Task<ReceiveEndpointReady> ready in readyTasks)
            {
                await ready.ConfigureAwait(false);
            }

            ReceiveEndpointReady[] endpointsReady = await Task.WhenAll(readyTasks).ConfigureAwait(false);

            return new HostReadyEvent(_host.Address, endpointsReady);
        }
    }
}