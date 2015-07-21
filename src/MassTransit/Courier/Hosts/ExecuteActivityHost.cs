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
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Util;


    public class ExecuteActivityHost<TActivity, TArguments> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        static readonly ILog _log = Logger.Get<ExecuteActivityHost<TActivity, TArguments>>();
        readonly ExecuteActivityFactory<TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly IPipe<ExecuteActivityContext<TArguments>> _executePipe;

        public ExecuteActivityHost(ExecuteActivityFactory<TArguments> activityFactory, Uri compensateAddress)
        {
            if (compensateAddress == null)
                throw new ArgumentNullException(nameof(compensateAddress));
            if (activityFactory == null)
                throw new ArgumentNullException(nameof(activityFactory));

            _compensateAddress = compensateAddress;
            _activityFactory = activityFactory;

            _executePipe = Pipe.New<ExecuteActivityContext<TArguments>>(x => x.UseFilter(new ExecuteActivityFilter<TArguments>()));
        }

        public ExecuteActivityHost(ExecuteActivityFactory<TArguments> activityFactory)
        {
            if (activityFactory == null)
                throw new ArgumentNullException(nameof(activityFactory));

            _activityFactory = activityFactory;

            _executePipe = Pipe.New<ExecuteActivityContext<TArguments>>(x => x.UseFilter(new ExecuteActivityFilter<TArguments>()));
        }

        public void Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("executeActivity");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                ArgumentType = TypeMetadataCache<TArguments>.ShortName,
            });
            if (_compensateAddress != null)
                scope.Add("compensateAddress", _compensateAddress);

            _executePipe.Probe(scope);
        }

        public async Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                ExecuteContext<TArguments> executeContext = new HostExecuteContext<TArguments>(HostMetadataCache.Host, _compensateAddress, context);

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Host: {0} Activity: {1} Executing: {2}", context.ReceiveContext.InputAddress, TypeMetadataCache<TActivity>.ShortName,
                        executeContext.TrackingNumber);
                }

                await _activityFactory.Execute(executeContext, _executePipe);

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName);

                await next.Send(context);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, ex);
                throw;
            }
        }
    }
}