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
namespace MassTransit.HttpTransport.Clients
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Pipeline;


    public class ClientContextFactory :
        IPipeContextFactory<ClientContext>
    {
        readonly IReceivePipe _receivePipe;

        public ClientContextFactory(IReceivePipe receivePipe)
        {
            _receivePipe = receivePipe;
        }

        public IPipeContextAgent<ClientContext> CreateContext(ISupervisor supervisor)
        {
            var client = new HttpClient();
            ClientContext clientContext = new HttpClientContext(client, _receivePipe, supervisor.Stopped);

            return supervisor.AddContext(clientContext);
        }

        public IActivePipeContextAgent<ClientContext> CreateActiveContext(ISupervisor supervisor, PipeContextHandle<ClientContext> context,
            CancellationToken cancellationToken = default)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ClientContext> CreateSharedConnection(Task<ClientContext> context, CancellationToken cancellationToken)
        {
            var clientContext = await context.ConfigureAwait(false);

            return new SharedHttpClientContext(clientContext, cancellationToken);
        }
    }
}