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
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ConcurrencyLimit;
    using Util;


    public class PartitionFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly long[] _attemptCount;
        readonly long[] _failureCount;
        readonly IHashGenerator _hashGenerator;
        readonly PartitionKeyProvider<ConsumeContext<TMessage>> _keyProvider;
        readonly int _partitionCount;
        readonly IFilter<ConsumeContext<TMessage>>[] _partitions;
        readonly long[] _successCount;

        public PartitionFilter(int partitionCount, PartitionKeyProvider<ConsumeContext<TMessage>> keyProvider, IHashGenerator hashGenerator)
        {
            _partitionCount = partitionCount;
            _keyProvider = keyProvider;
            _hashGenerator = hashGenerator;

            var mediator = new Mediator<IConcurrencyLimitFilter>();

            _partitions = Enumerable.Range(0, partitionCount)
                .Select(index => new ConcurrencyLimitFilter<ConsumeContext<TMessage>>(1, mediator))
                .Cast<IFilter<ConsumeContext<TMessage>>>()
                .ToArray();

            _attemptCount = new long[partitionCount];
            _successCount = new long[partitionCount];
            _failureCount = new long[partitionCount];
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var key = _keyProvider(context);
            if (key == null)
                throw new InvalidOperationException("The key cannot be null");

            var hash = _hashGenerator.Hash(key);

            var partitionId = hash % _partitionCount;

            try
            {
                Interlocked.Increment(ref _attemptCount[partitionId]);

                await _partitions[partitionId].Send(context, next);

                Interlocked.Increment(ref _successCount[partitionId]);
            }
            catch
            {
                Interlocked.Increment(ref _failureCount[partitionId]);
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("partition");
            scope.Add("partitionCount", _partitionCount);
            for (int i = 0; i < _partitionCount; i++)
            {
                var partitionScope = scope.CreateScope($"partition-{i}");
                partitionScope.Set(new
                {
                    AttemptCount = _attemptCount[i],
                    SuccessCount = _successCount[i],
                    FailureCount = _failureCount[i]
                });
                _partitions[i].Probe(partitionScope);
            }
        }
    }
}