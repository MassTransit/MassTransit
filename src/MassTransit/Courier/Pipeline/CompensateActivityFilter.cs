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
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Util;


    /// <summary>
    /// Compensates an activity as part of an activity execute host pipe
    /// </summary>
    /// <typeparam name="TLog"></typeparam>
    /// <typeparam name="TActivity"></typeparam>
    public class CompensateActivityFilter<TActivity, TLog> :
        IFilter<CompensateActivityContext<TActivity, TLog>>
        where TLog : class
        where TActivity : class, CompensateActivity<TLog>
    {
        static readonly ILog _log = Logger.Get<CompensateActivityFilter<TActivity, TLog>>();

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("compensate");
        }

        public async Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Compensating: {0}", context.TrackingNumber);

            try
            {
                try
                {
                    var result = await context.Activity.Compensate(context).ConfigureAwait(false);

                    await result.Evaluate().ConfigureAwait(false);

                    await next.Send(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var result = context.Failed(ex);

                    await result.Evaluate().ConfigureAwait(false);
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