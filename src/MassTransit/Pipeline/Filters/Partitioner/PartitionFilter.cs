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
namespace MassTransit.Pipeline.Filters.Partitioner
{
    using System.Threading.Tasks;


    public class PartitionFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IPartitioner<ConsumeContext<TMessage>> _partitioner;

        public PartitionFilter(PartitionKeyProvider<ConsumeContext<TMessage>> keyProvider, IPartitioner partitioner)
        {
            _partitioner = partitioner.GetPartitioner(keyProvider);
        }

        Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            return _partitioner.Send(context, next);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("partition");
            _partitioner.Probe(scope);
        }
    }
}