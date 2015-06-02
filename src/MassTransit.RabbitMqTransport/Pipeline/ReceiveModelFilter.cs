// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Contexts;
    using MassTransit.Pipeline;


    /// <summary>
    /// Creates a receiving model context using the connection
    /// </summary>
    public class ReceiveModelFilter :
        IFilter<ConnectionContext>
    {
        readonly IPipe<ModelContext> _pipe;

        public ReceiveModelFilter(IPipe<ModelContext> pipe)
        {
            _pipe = pipe;
        }

        public async Task Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            using (var modelContext = new RabbitMqModelContext(context, context.CancellationToken))
            {
                await _pipe.Send(modelContext).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _pipe.Visit(x));
        }
    }
}