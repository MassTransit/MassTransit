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
namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Configurators;
    using PipeBuilders;
    using Pipeline.Filters.Partitioner;


    public class PartitionerPipeSpecification<TMessage> :
        IPipeSpecification<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly PartitionKeyProvider<ConsumeContext<TMessage>> _keyProvider;
        readonly int _partitionCount;

        public PartitionerPipeSpecification(PartitionKeyProvider<ConsumeContext<TMessage>> keyProvider, int partitionCount)
        {
            _keyProvider = keyProvider;
            _partitionCount = partitionCount;
        }

        public void Apply(IPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            IHashGenerator hashGenerator = new Murmur3UnsafeHashGenerator();

            builder.AddFilter(new PartitionFilter<TMessage>(_partitionCount, _keyProvider, hashGenerator));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_keyProvider == null)
                yield return this.Failure("KeyProvider", "must not be null");
            if (_partitionCount < 1)
                yield return this.Failure("PartitionCount", "must be >= 1");
        }
    }
}