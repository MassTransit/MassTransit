// Copyright 2007-2014 Chris Patterson
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
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using Util;


    public class ExecuteActivityHost<TActivity, TArguments> :
        IConsumer<RoutingSlip>
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityFactory<TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly ILog _log = Logger.Get<ExecuteActivityHost<TActivity, TArguments>>();

        public ExecuteActivityHost(Uri compensateAddress, ExecuteActivityFactory<TArguments> activityFactory)
        {
            if (compensateAddress == null)
                throw new ArgumentNullException("compensateAddress");
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");

            _compensateAddress = compensateAddress;
            _activityFactory = activityFactory;
        }

        public ExecuteActivityHost(ExecuteActivityFactory<TArguments> activityFactory)
        {
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");

            _activityFactory = activityFactory;
        }

        async Task IConsumer<RoutingSlip>.Consume(ConsumeContext<RoutingSlip> context)
        {
            Execution<TArguments> execution = new HostExecution<TArguments>(HostMetadataCache.Host, _compensateAddress, context);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Host: {0} Executing: {1}", context.ReceiveContext.InputAddress, execution.TrackingNumber);

            try
            {
                Exception exception = null;
                try
                {
                    ExecutionResult result = await _activityFactory.ExecuteActivity(execution);

                    await result.Evaluate();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }

                if (exception != null)
                {
                    ExecutionResult result = execution.Faulted(exception);

                    await result.Evaluate();
                }
            }
            catch (Exception ex)
            {
                _log.Error("The activity threw an unexpected exception", ex);
            }
        }
    }
}