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


    /// <summary>
    /// Executes an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TArguments"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public class ExecuteActivityFilter<TActivity, TArguments> :
        IFilter<RequestContext<ExecuteActivityContext<TActivity, TArguments>>>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        static readonly ILog _log = Logger.Get<ExecuteActivityFilter<TActivity, TArguments>>();

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("execute");
        }

        public async Task Send(RequestContext<ExecuteActivityContext<TActivity, TArguments>> context,
            IPipe<RequestContext<ExecuteActivityContext<TActivity, TArguments>>> next)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Executing: {0}", context.Request.TrackingNumber);

            var result = await context.Request.Activity.Execute(context.Request).ConfigureAwait(false);

            context.TrySetResult(result);

            await next.Send(context).ConfigureAwait(false);
        }
    }
}