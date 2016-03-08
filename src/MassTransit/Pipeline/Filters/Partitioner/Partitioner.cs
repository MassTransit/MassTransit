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
    using System.Threading.Tasks;


    public class Partitioner :
        IPartitioner
    {
        readonly IHashGenerator _hashGenerator;
        readonly string _id;
        readonly int _partitionCount;
        readonly Partition[] _partitions;

        public Partitioner(int partitionCount, IHashGenerator hashGenerator)
        {
            _id = NewId.NextGuid().ToString("N");

            _partitionCount = partitionCount;
            _hashGenerator = hashGenerator;
            _partitions = Enumerable.Range(0, partitionCount)
                .Select(index => new Partition(index))
                .ToArray();
        }

        IPartitioner<T> IPartitioner.GetPartitioner<T>(PartitionKeyProvider<T> keyProvider)
        {
            return new ContextPartitioner<T>(this, keyProvider);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("partitioner");
            scope.Add("id", _id);
            scope.Add("partitionCount", _partitionCount);

            for (var i = 0; i < _partitions.Length; i++)
            {
                _partitions[i].Probe(scope);
            }
        }

        Task Send<T>(byte[] key, T context, IPipe<T> next) where T : class, PipeContext
        {
            var hash = _hashGenerator.Hash(key);

            var partitionId = hash % _partitionCount;

            return _partitions[partitionId].Send(context, next);
        }


        class ContextPartitioner<T> :
            IPartitioner<T>
            where T : class, PipeContext
        {
            readonly PartitionKeyProvider<T> _keyProvider;
            readonly Partitioner _partitioner;

            public ContextPartitioner(Partitioner partitioner, PartitionKeyProvider<T> keyProvider)
            {
                _partitioner = partitioner;
                _keyProvider = keyProvider;
            }

            public Task Send(T context, IPipe<T> next)
            {
                byte[] key = _keyProvider(context);
                if (key == null)
                    throw new InvalidOperationException("The key cannot be null");

                return _partitioner.Send(key, context, next);
            }

            public void Probe(ProbeContext context)
            {
                _partitioner.Probe(context);
            }
        }
    }
}