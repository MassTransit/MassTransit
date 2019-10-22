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
namespace MassTransit.HttpTransport.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;


    public class HttpHostContextFactory :
        IPipeContextFactory<HttpHostContext>
    {
        readonly IHttpHostConfiguration _configuration;

        public HttpHostContextFactory(IHttpHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        IPipeContextAgent<HttpHostContext> IPipeContextFactory<HttpHostContext>.CreateContext(ISupervisor supervisor)
        {

            HttpHostContext hostContext = new KestrelHttpHostContext(_configuration, supervisor.Stopped);

            hostContext.GetOrAddPayload(() => _configuration);

            IPipeContextAgent<HttpHostContext> contextHandle = supervisor.AddContext(hostContext);

            return contextHandle;
        }

        IActivePipeContextAgent<HttpHostContext> IPipeContextFactory<HttpHostContext>.CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<HttpHostContext> context,
            CancellationToken cancellationToken)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        async Task<HttpHostContext> CreateSharedConnection(Task<HttpHostContext> context, CancellationToken cancellationToken)
        {
            var connectionContext = await context.ConfigureAwait(false);

            var sharedConnection = new SharedHttpHttpHostContext(connectionContext, cancellationToken);

            return sharedConnection;
        }
    }
}
