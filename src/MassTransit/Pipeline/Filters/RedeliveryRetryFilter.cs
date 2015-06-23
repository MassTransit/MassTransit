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
    using Monitoring.Introspection;
    using Policies;

    /// <summary>
    /// Uses the message redelivery mechanism, if available, to delay a retry without blocking message delivery
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedeliveryRetryFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly IRetryPolicy _retryPolicy;

        public RedeliveryRetryFilter(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("retry");
            scope.Add("type", "redelivery");


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
                        throw new ContextException("The message redelivery context was not available to delay the message", exception);

                    TimeSpan delay;
                    using (IRetryContext retryContext = _retryPolicy.GetRetryContext())
                    {
                        retryContext.CanRetry(exception, out delay);
                    }

                    await schedulerContext.ScheduleRedelivery(delay);
                }
                catch (Exception ex)
                {
                    throw new ContextException("The message delivery could not be rescheduled", new AggregateException(ex, exception));
                }
            }
        }
    }
}