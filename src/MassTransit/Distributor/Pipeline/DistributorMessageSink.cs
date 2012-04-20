// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using Messages;

    public class DistributorMessageSink<TMessage> :
        IPipelineSink<IConsumeContext<TMessage>>
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

        public IEnumerable<Action<IConsumeContext<TMessage>>> Enumerate(IConsumeContext<TMessage> context)
        {
            return _workerAvailability.GetWorker(context, Handle, _workerSelector)
                .DefaultIfEmpty(RetryLaterHandler);
        }

        public bool Inspect(IPipelineInspector inspector)
        {
            return inspector.Inspect(this);
        }

        void RetryLaterHandler(IConsumeContext<TMessage> context)
        {
            context.RetryLater();
        }

        IEnumerable<Action<IConsumeContext<TMessage>>> Handle(IWorkerInfo<TMessage> worker)
        {
            yield return context =>
                {
                    context.BaseContext.NotifyConsume(context,
                        typeof(DistributorMessageSink<TMessage>).ToShortTypeName(), null);

                    IEndpoint endpoint = context.Bus.GetEndpoint(worker.DataUri);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Sending {0}[{1}] to {2}", typeof(TMessage).ToShortTypeName(),
                            context.MessageId, worker.DataUri);

                    var distributed = new Distributed<TMessage>(context.Message, context.ResponseAddress);

                    endpoint.Send(distributed, x =>
                        {
                            x.SetRequestId(context.RequestId);
                            x.SetConversationId(context.ConversationId);
                            x.SetCorrelationId(context.CorrelationId);
                            x.SetSourceAddress(context.SourceAddress);
                            x.SetDestinationAddress(context.DestinationAddress);
                            x.SetResponseAddress(context.ResponseAddress);
                            x.SetFaultAddress(context.FaultAddress);
                            x.SetNetwork(context.Network);
                            if (context.ExpirationTime.HasValue)
                                x.SetExpirationTime(context.ExpirationTime.Value);

                            context.Headers.Each(header => x.SetHeader(header.Key, header.Value));

                            x.SetHeader("mt.worker.uri", worker.DataUri.ToString());
                        });
                };
        }
    }
}