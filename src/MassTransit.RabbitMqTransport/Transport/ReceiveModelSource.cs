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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Payloads;


    public class ReceiveModelSource :
        Agent,
        IAgent<ModelContext>
    {
        readonly ModelContext _context;
        readonly ReceiveContext _receiveContext;

        public ReceiveModelSource(ModelContext context, ReceiveContext receiveContext)
        {
            _context = context;
            _receiveContext = receiveContext;
        }

        public Task Send(IPipe<ModelContext> pipe, CancellationToken cancellationToken = default(CancellationToken))
        {
            ModelContext modelContext = new SharedModelContext(_context, new PayloadCacheScope(_context), cancellationToken);
            
            modelContext.GetOrAddPayload(() => _receiveContext);

            return pipe.Send(modelContext);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("receiveModelContext");
        }
    }
}