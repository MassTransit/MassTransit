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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Topology.Conventions;


    public class SetPartitionKeyFilter<T> :
        IFilter<ServiceBusSendContext<T>>
        where T : class
    {
        readonly IMessagePartitionKeyFormatter<T> _partitionKeyFormatter;

        public SetPartitionKeyFilter(IMessagePartitionKeyFormatter<T> partitionKeyFormatter)
        {
            _partitionKeyFormatter = partitionKeyFormatter;
        }

        public Task Send(ServiceBusSendContext<T> context, IPipe<ServiceBusSendContext<T>> next)
        {
            var partitionKey = _partitionKeyFormatter.FormatPartitionKey(context);

            if (!string.IsNullOrWhiteSpace(partitionKey))
                context.PartitionKey = partitionKey;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("setPartitionKey");
        }
    }
}