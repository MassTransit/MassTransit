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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Monitoring.Introspection;
    using Pipeline;


    public class ReceiveFaultTracker :
        IFilter<ReceiveContext>
    {
        readonly IPipe<ReceiveContext> _faultPipe;
        readonly IInboundMessageTracker _tracker;

        public ReceiveFaultTracker(IInboundMessageTracker tracker, IPipe<ReceiveContext> faultPipe)
        {
            _tracker = tracker;
            _faultPipe = faultPipe;
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            var messageId = context.TransportHeaders.Get<string>("MessageId");

            Exception exception;
            IEnumerable<Action> faultActions;
            if (_tracker.IsRetryLimitExceeded(messageId, out exception, out faultActions))
            {
                // execute fault actions

                await _faultPipe.Send(context);

                _tracker.MessageWasMovedToErrorQueue(messageId);
            }
            else
            {
                try
                {
                    await next.Send(context);

                    _tracker.MessageWasReceivedSuccessfully(messageId);
                }
                catch (SerializationException sex)
                {
                    _tracker.IncrementRetryCount(messageId, sex);

                    // come on roslyn
                    _faultPipe.Send(context).Wait(context.CancellationToken);

                    _tracker.MessageWasMovedToErrorQueue(messageId);
                }
                catch (Exception ex)
                {
                    if (_tracker.IncrementRetryCount(messageId, ex))
                    {
                        // come on roslyn
                        _faultPipe.Send(context).Wait(context.CancellationToken);

                        _tracker.MessageWasMovedToErrorQueue(messageId);
                    }
                }
            }
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}