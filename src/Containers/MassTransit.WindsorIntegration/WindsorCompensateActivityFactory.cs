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
namespace MassTransit.WindsorIntegration
{
    using System.Threading.Tasks;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Lifestyle;
    using Courier;
    using Courier.Hosts;
    using GreenPipes;
    using Logging;
    using Util;


    /// <summary>
    /// A factory to create an activity from Windsor, that manages the lifetime scope of the activity
    /// </summary>
    /// <typeparam name="TActivity"></typeparam>
    /// <typeparam name="TLog"></typeparam>
    public class WindsorCompensateActivityFactory<TActivity, TLog> :
        CompensateActivityFactory<TActivity, TLog>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        static readonly ILog _log = Logger.Get<WindsorCompensateActivityFactory<TActivity, TLog>>();
        readonly IKernel _kernel;

        public WindsorCompensateActivityFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public async Task<ResultContext<CompensationResult>> Compensate(CompensateContext<TLog> context,
            IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> next)
        {
            using (_kernel.RequireScope())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("CompensateActivityFactory: Compensating: {0}", TypeMetadataCache<TActivity>.ShortName);

                var activity = _kernel.Resolve<TActivity>(new {context.Log, context.ConsumeContext});
                if (activity == null)
                {
                    throw new ConsumerException($"Unable to resolve activity type '{TypeMetadataCache<TActivity>.ShortName}'.");
                }

                var activityContext = new HostCompensateActivityContext<TActivity, TLog>(activity, context);

                return await next.Send(activityContext).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("windsor");
        }
    }
}