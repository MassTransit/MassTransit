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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;


    public class CancelRequestTimeoutActivity<TInstance, TData, TRequest, TResponse> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly Request<TInstance, TRequest, TResponse> _request;

        public CancelRequestTimeoutActivity(Request<TInstance, TRequest, TResponse> request)
        {
            _request = request;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("cancelRequest");
        }

        async Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeContext consumeContext = context.CreateConsumeContext();

            Guid? requestId = _request.GetRequestId(context.Instance);
            if (requestId.HasValue && _request.Settings.Timeout > TimeSpan.Zero)
            {
                MessageSchedulerContext schedulerContext;
                if (_request.Settings.SchedulingServiceAddress != null)
                {
                    var scheduleEndpoint = await consumeContext.GetSendEndpoint(_request.Settings.SchedulingServiceAddress).ConfigureAwait(false);

                    await scheduleEndpoint.CancelScheduledSend(requestId.Value).ConfigureAwait(false);
                }
                else if (consumeContext.TryGetPayload(out schedulerContext))
                    await schedulerContext.CancelScheduledSend(consumeContext.ReceiveContext.InputAddress, requestId.Value).ConfigureAwait(false);
                else
                    throw new ConfigurationException("A scheduler was not available to cancel the scheduled request timeout");
            }
        }
    }
}