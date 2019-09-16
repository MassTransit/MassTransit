// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Pipes;
    using Util;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaMessageSplitFilter<TSaga, TMessage> :
        IFilter<SagaConsumeContext<TSaga, TMessage>>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly IFilter<ConsumeContext<TMessage>> _next;

        public SagaMessageSplitFilter(IFilter<ConsumeContext<TMessage>> next)
        {
            _next = next;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("split");
            scope.Set(new
            {
                MessageType = TypeMetadataCache<TMessage>.ShortName
            });

            _next.Probe(scope);
        }

        [DebuggerNonUserCode]
        public Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            var mergePipe = new SagaMessageMergePipe<TSaga, TMessage>(next, context);

            return _next.Send(context, mergePipe);
        }
    }
}