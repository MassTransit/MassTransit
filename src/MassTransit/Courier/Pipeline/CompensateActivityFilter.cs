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
namespace MassTransit.Courier.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Microsoft.Extensions.Logging;


    /// <summary>
    /// Compensates an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public class CompensateActivityFilter<TActivity, TLog> :
        IFilter<RequestContext<CompensateActivityContext<TActivity, TLog>>>
        where TLog : class
        where TActivity : class, CompensateActivity<TLog>
    {
        static readonly ILogger _logger = Logger.Get<CompensateActivityFilter<TActivity, TLog>>();

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("compensate");
        }

        public async Task Send(RequestContext<CompensateActivityContext<TActivity, TLog>> context,
            IPipe<RequestContext<CompensateActivityContext<TActivity, TLog>>> next)
        {
            _logger.LogDebug("Compensating: {0}", context.Request.TrackingNumber);

            var result = await context.Request.Activity.Compensate(context.Request).ConfigureAwait(false);

            context.TrySetResult(result);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
