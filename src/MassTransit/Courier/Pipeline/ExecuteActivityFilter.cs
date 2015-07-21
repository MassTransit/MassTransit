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
namespace MassTransit.Courier.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Util;


    /// <summary>
    /// Executes an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TArguments"></typeparam>
    public class ExecuteActivityFilter<TArguments> :
        IFilter<ExecuteActivityContext<TArguments>>
        where TArguments : class
    {
        static readonly ILog _log = Logger.Get<ExecuteActivityFilter<TArguments>>();

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("execute");
        }

        public async Task Send(ExecuteActivityContext<TArguments> context, IPipe<ExecuteActivityContext<TArguments>> next)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Executing: {0}", context.TrackingNumber);

            try
            {
                try
                {
                    ExecutionResult result = await context.Activity.Execute(context);

                    await result.Evaluate();

                    await next.Send(context);
                }
                catch (Exception ex)
                {
                    ExecutionResult result = context.Faulted(ex);

                    await result.Evaluate();
                }

            }
            catch (Exception ex)
            {
                _log.Error($"The activity {TypeMetadataCache.GetShortName(context.Activity.GetType())} threw an exception", ex);

                throw;
            }
        }
    }
}