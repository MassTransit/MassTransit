// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.Pipeline
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Sinks;
    using Messages;
    using Util;


    public class DistributorMessageSink<TMessage> :
        IConsumeFilter<TMessage>
        where TMessage : class
    {
        readonly ILog _log = Logger.Get<DistributorMessageSink<TMessage>>();

        readonly IWorkerAvailability<TMessage> _workerAvailability;
        readonly IWorkerSelector<TMessage> _workerSelector;

        public DistributorMessageSink(IWorkerAvailability<TMessage> workerAvailability,
            IWorkerSelector<TMessage> workerSelector)
        {
            _workerAvailability = workerAvailability;
            _workerSelector = workerSelector;
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();

            IWorkerInfo<TMessage> worker = _workerAvailability.GetWorker(context, _workerSelector);
            if (worker == null)
            {
                context.RetryLater();
                return;
            }

            IEndpoint endpoint = context.GetEndpoint(worker.DataUri);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Sending {0}[{1}] to {2}", TypeMetadataCache<TMessage>.ShortName,
                    context.MessageId, worker.DataUri);
            }

            var distributed = new Distributed<TMessage>(context.Message, context.ResponseAddress);

            endpoint.Send(distributed, x =>
            {
//                x.SetRequestId(context.RequestId);
//                x.SetConversationId(context.ConversationId);
//                x.SetCorrelationId(context.CorrelationId);
                x.SetSourceAddress(context.SourceAddress);
                x.SetDestinationAddress(context.DestinationAddress);
                x.SetResponseAddress(context.ResponseAddress);
                x.SetFaultAddress(context.FaultAddress);
                if (context.ExpirationTime.HasValue)
                    x.SetExpirationTime(context.ExpirationTime.Value);

//                context.Headers.Each(header => x.SetHeader(header.Key, header.Value));

                x.SetHeader("mt.worker.uri", worker.DataUri.ToString());
            });


            timer.Stop();
            context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TMessage>.ShortName,
                TypeMetadataCache<DistributorMessageSink<TMessage>>.ShortName);

            await next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }
    }
}