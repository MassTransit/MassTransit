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
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Util;


    public class ExecuteActivityHost<TActivity, TArguments> :
        IConsumer<RoutingSlip>
        where TActivity : ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ExecuteActivityFactory<TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly ILog _log = Logger.Get<ExecuteActivityHost<TActivity, TArguments>>();
        IPipe<ExecuteActivityContext<TArguments>> _executePipe;

        public ExecuteActivityHost(Uri compensateAddress, ExecuteActivityFactory<TArguments> activityFactory)
        {
            if (compensateAddress == null)
                throw new ArgumentNullException("compensateAddress");
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");

            _compensateAddress = compensateAddress;
            _activityFactory = activityFactory;

            _executePipe = Pipe.New<ExecuteActivityContext<TArguments>>(x => x.Filter(new ExecuteActivityFilter<TArguments>()));
        }

        public ExecuteActivityHost(ExecuteActivityFactory<TArguments> activityFactory)
        {
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");

            _activityFactory = activityFactory;
        }

        Task IConsumer<RoutingSlip>.Consume(ConsumeContext<RoutingSlip> context)
        {
            Execution<TArguments> execution = new HostExecution<TArguments>(HostMetadataCache.Host, _compensateAddress, context);

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Host: {0} Activity: {1} Executing: {2}", context.ReceiveContext.InputAddress, TypeMetadataCache<TActivity>.ShortName,
                    execution.TrackingNumber);
            }

            return _activityFactory.Execute(execution, _executePipe);
        }
    }
}