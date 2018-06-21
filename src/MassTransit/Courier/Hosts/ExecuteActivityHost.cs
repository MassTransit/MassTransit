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


    public class ExecuteActivityHost<TActivity, TArguments> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        static readonly ILog _log = Logger.Get<ExecuteActivityHost<TActivity, TArguments>>();
        readonly ExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> _executePipe;

        public ExecuteActivityHost(ExecuteActivityFactory<TActivity, TArguments> activityFactory,
            IPipe<RequestContext> executePipe, Uri compensateAddress)
        {
            if (compensateAddress == null)
                throw new ArgumentNullException(nameof(compensateAddress));
            if (activityFactory == null)
                throw new ArgumentNullException(nameof(activityFactory));

            _compensateAddress = compensateAddress;
            _activityFactory = activityFactory;
            _executePipe = executePipe.CreateRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult>();
        }

        public ExecuteActivityHost(ExecuteActivityFactory<TActivity, TArguments> activityFactory,
            IPipe<RequestContext> executePipe)
        {
            if (activityFactory == null)
                throw new ArgumentNullException(nameof(activityFactory));

            _activityFactory = activityFactory;
            _executePipe = executePipe.CreateRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult>();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("executeActivity");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                ArgumentType = TypeMetadataCache<TArguments>.ShortName
            });
            if (_compensateAddress != null)
                scope.Add("compensateAddress", _compensateAddress);

            _executePipe.Probe(scope);
        }

        public async Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                ExecuteContext<TArguments> executeContext = new HostExecuteContext<TArguments>(HostMetadataCache.Host, _compensateAddress, context);

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Host: {0} Activity: {1} Executing: {2}", context.ReceiveContext.InputAddress, TypeMetadataCache<TActivity>.ShortName,
                        executeContext.TrackingNumber);
                }

                await Task.Yield();

                try
                {
                    var result = await _activityFactory.Execute(executeContext, _executePipe).Result().ConfigureAwait(false);

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var result = executeContext.Faulted(ex);

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
    }
}