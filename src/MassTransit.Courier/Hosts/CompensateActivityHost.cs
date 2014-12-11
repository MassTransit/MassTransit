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


    public class CompensateActivityHost<TActivity, TLog> :
        IConsumer<RoutingSlip>
        where TActivity : CompensateActivity<TLog>
        where TLog : class
    {
        readonly CompensateActivityFactory<TLog> _activityFactory;
        readonly HostInfo _host;
        readonly ILog _log = Logger.Get<CompensateActivityHost<TActivity, TLog>>();

        public CompensateActivityHost(CompensateActivityFactory<TLog> activityFactory)
        {
            _activityFactory = activityFactory;
            _host = HostMetadataCache.Host;
        }

        async Task IConsumer<RoutingSlip>.Consume(ConsumeContext<RoutingSlip> context)
        {
            Compensation<TLog> compensation = new HostCompensation<TLog>(HostMetadataCache.Host, context);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Host: {0} Compensating: {1}", context.ReceiveContext.InputAddress, compensation.TrackingNumber);

            CompensationResult result;
            try
            {
                // TODO: MassTransit Async Support
                result = _activityFactory.CompensateActivity(compensation).Result;
            }
            catch (Exception ex)
            {
                result = compensation.Failed(ex);
            }

            await result.Evaluate();
        }
    }
}