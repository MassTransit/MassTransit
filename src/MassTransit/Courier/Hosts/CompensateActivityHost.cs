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
namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using Logging;
    using Util;


    public class CompensateActivityHost<TActivity, TLog> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        static readonly ILog _log = Logger.Get<CompensateActivityHost<TActivity, TLog>>();
        readonly CompensateActivityFactory<TActivity, TLog> _activityFactory;
        readonly IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> _compensatePipe;

        public CompensateActivityHost(CompensateActivityFactory<TActivity, TLog> activityFactory,
            IPipe<RequestContext> compensatePipe)
        {
            _activityFactory = activityFactory;
            _compensatePipe = compensatePipe.CreateRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult>();
        }

        public async Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                CompensateContext<TLog> compensateContext = new HostCompensateContext<TLog>(HostMetadataCache.Host, context);

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Host: {0} Activity: {1} Compensating: {2}", context.ReceiveContext.InputAddress, TypeMetadataCache<TActivity>.ShortName,
                        compensateContext.TrackingNumber);
                }

                await Task.Yield();

                try
                {
                    var result = await _activityFactory.Compensate(compensateContext, _compensatePipe).Result().ConfigureAwait(false);

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var result = compensateContext.Failed(ex);

                    await result.Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"The activity {TypeMetadataCache<TActivity>.ShortName} threw an exception", ex);

                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, ex).ConfigureAwait(false);

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("compensateActivity");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                LogType = TypeMetadataCache<TLog>.ShortName
            });

            _compensatePipe.Probe(scope);
        }
    }
}