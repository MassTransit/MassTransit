// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Policies;
    using Scheduling;


    public class DelayedRetryFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly IRetryPolicy _retryPolicy;

        public DelayedRetryFilter(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            Exception exception = null;
            try
            {
                await next.Send(context);
            }
            catch (Exception ex)
            {
                if (!_retryPolicy.CanRetry(ex))
                    throw;

                exception = ex;
            }

            if (exception != null)
            {
                try
                {
                    MessageRedeliveryContext schedulerContext;
                    if (!context.TryGetPayload(out schedulerContext))
                        throw new ContextException("The scheduler context was not available to delay the message", exception);

                    TimeSpan delay;
                    using (IRetryContext retryContext = _retryPolicy.GetRetryContext())
                    {
                        retryContext.CanRetry(exception, out delay);
                    }

                    await schedulerContext.ScheduleRedelivery(delay);
                }
                catch (Exception ex)
                {
                    throw new ContextException("The scheduler could not reschedule the message delivery", new AggregateException(ex, exception));
                }
            }
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}