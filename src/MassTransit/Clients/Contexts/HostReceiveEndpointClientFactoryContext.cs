// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Clients.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class HostReceiveEndpointClientFactoryContext :
        ReceiveEndpointClientFactoryContext,
        IAsyncDisposable
    {
        readonly HostReceiveEndpointHandle _receiveEndpointHandle;

        public HostReceiveEndpointClientFactoryContext(HostReceiveEndpointHandle receiveEndpointHandle, ReceiveEndpointReady receiveEndpointReady,
            RequestTimeout defaultTimeout = default)
            : base(receiveEndpointReady, defaultTimeout)
        {
            _receiveEndpointHandle = receiveEndpointHandle;
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return _receiveEndpointHandle.StopAsync(cancellationToken);
        }
    }
}