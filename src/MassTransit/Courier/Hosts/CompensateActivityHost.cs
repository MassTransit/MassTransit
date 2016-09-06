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
namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Util;


    public class CompensateActivityHost<TActivity, TLog> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : CompensateActivity<TLog>
        where TLog : class
    {
        static readonly ILog _log = Logger.Get<CompensateActivityHost<TActivity, TLog>>();
        readonly CompensateActivityFactory<TLog> _activityFactory;
        readonly IPipe<CompensateActivityContext<TLog>> _compensatePipe;

        public CompensateActivityHost(CompensateActivityFactory<TLog> activityFactory)
        {
            _activityFactory = activityFactory;

            _compensatePipe = Pipe.New<CompensateActivityContext<TLog>>(x => x.UseFilter(new CompensateActivityFilter<TLog>()));
        }

        public async Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                CompensateContext<TLog> compensateContext = new HostCompensateContext<TLog>(HostMetadataCache.Host, context);

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Host: {0} Activity: {1} Compensating: {2}", context.ReceiveContext.InputAddress, TypeMetadataCache<TActivity>.ShortName,
                        compensateContext.TrackingNumber);
                }

                await Task.Yield();

                await _activityFactory.Compensate(compensateContext, _compensatePipe).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, ex).ConfigureAwait(false);
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("compensateActivity");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                LogType = TypeMetadataCache<TLog>.ShortName,
            });

            _compensatePipe.Probe(scope);
        }
    }
}