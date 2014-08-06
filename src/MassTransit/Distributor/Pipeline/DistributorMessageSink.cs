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
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Messages;
    using Util;


    public class DistributorMessageSink<TMessage> :
        IFilter<ConsumeContext<TMessage>>
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

            ISendEndpoint endpoint = context.GetSendEndpoint(worker.DataUri);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Sending {0}[{1}] to {2}", TypeMetadataCache<TMessage>.ShortName,
                    context.MessageId, worker.DataUri);
            }

            var distributed = new Distributed<TMessage>(context.Message, context.ResponseAddress);

            var pipe = new WorkerPipe<TMessage>(context, worker.DataUri.ToString());

            await endpoint.Send(distributed, pipe);

            timer.Stop();
            context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TMessage>.ShortName,
                TypeMetadataCache<DistributorMessageSink<TMessage>>.ShortName);

            await next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }


        class WorkerPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly ConsumeContext _context;
            readonly string _dataUri;

            public WorkerPipe(ConsumeContext context, string dataUri)
            {
                _context = context;
                _dataUri = dataUri;
            }

            public async Task Send(SendContext<T> context)
            {
                context.SourceAddress = _context.SourceAddress;
                context.DestinationAddress = _context.DestinationAddress;
                context.ResponseAddress = _context.ResponseAddress;
                context.FaultAddress = _context.FaultAddress;


                if (_context.ExpirationTime.HasValue)
                    context.TimeToLive = _context.ExpirationTime - DateTime.UtcNow;


                //                context.Headers.Each(header => x.SetHeader(header.Key, header.Value));

                context.ContextHeaders.Set("mt.worker.uri", _dataUri);
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return inspector.Inspect(this);
            }
        }
    }
}