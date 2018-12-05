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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Creates a <see cref="ModelContext"/> and sends it to the receive endpoint pipe
    /// </summary>
    public class ReceiveEndpointFilter :
        IFilter<ConnectionContext>
    {
        readonly IPipe<ModelContext> _pipe;

        public ReceiveEndpointFilter(IPipe<ModelContext> pipe)
        {
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("receiveEndpoint");

            _pipe.Probe(scope);
        }

        async Task IFilter<ConnectionContext>.Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            var modelContext = await context.CreateModelContext(context.CancellationToken).ConfigureAwait(false);

            try
            {
                await _pipe.Send(modelContext).ConfigureAwait(false);
            }
            finally
            {
                if (modelContext is IAsyncDisposable disposable)
                    await disposable.DisposeAsync(CancellationToken.None).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }
    }
}