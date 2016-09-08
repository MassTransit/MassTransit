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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


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

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("retry");
            scope.Add("type", "redelivery");
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var retryContext = _retryPolicy.CreatePolicyContext(context);

            try
            {
                await next.Send(retryContext.Context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                RetryContext<ConsumeContext<T>> nextRetryContext;
                if (!retryContext.CanRetry(ex, out nextRetryContext))
                    throw;

                try
                {
                    MessageRedeliveryContext redeliveryContext;
                    if (!context.TryGetPayload(out redeliveryContext))
                        throw new ContextException("The message redelivery context was not available to delay the message", ex);

                    if (!nextRetryContext.Delay.HasValue)
                        throw new ContextException("The message retry policy did not provide a delay for redelivery", ex);

                    await redeliveryContext.ScheduleRedelivery(nextRetryContext.Delay.Value).ConfigureAwait(false);
                }
                catch (Exception redeliveryException)
                {
                    throw new ContextException("The message delivery could not be rescheduled", new AggregateException(redeliveryException, ex));
                }
            }
        }
    }
}