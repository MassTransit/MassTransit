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
namespace MassTransit.Courier.Results
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Extensions;


    class CompensatedCompensationResult<TLog> :
        CompensationResult
        where TLog : class
    {
        readonly CompensateContext<TLog> _compensateContext;
        readonly CompensateLog _compensateLog;
        readonly TimeSpan _duration;
        readonly IRoutingSlipEventPublisher _publisher;
        readonly RoutingSlip _routingSlip;

        public CompensatedCompensationResult(CompensateContext<TLog> compensateContext, IRoutingSlipEventPublisher publisher, CompensateLog compensateLog,
            RoutingSlip routingSlip)
        {
            _compensateContext = compensateContext;
            _publisher = publisher;
            _compensateLog = compensateLog;
            _routingSlip = routingSlip;
            _duration = _compensateContext.ElapsedTime;
        }

        public async Task Evaluate()
        {
            RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            RoutingSlip routingSlip = builder.Build();

             _publisher.PublishRoutingSlipActivityCompensated(_compensateContext.ActivityName, _compensateContext.ExecutionId,
                _compensateContext.StartTimestamp, _duration, _routingSlip.Variables, _compensateLog.Data);

            if (HasMoreCompensations(routingSlip))
            {
                ISendEndpoint endpoint = await _compensateContext.GetSendEndpoint(routingSlip.GetNextCompensateAddress());

                 _compensateContext.ConsumeContext.Forward(endpoint, routingSlip);
            }
            else
            {
                DateTime faultedTimestamp = _compensateContext.StartTimestamp + _duration;
                TimeSpan faultedDuration = faultedTimestamp - _routingSlip.CreateTimestamp;

                 _publisher.PublishRoutingSlipFaulted(faultedTimestamp, faultedDuration, _routingSlip.Variables,
                    _routingSlip.ActivityExceptions.ToArray());
            }
        }

        bool HasMoreCompensations(RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs != null && routingSlip.CompensateLogs.Count > 0;
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.CompensateLogs.SkipLast());
        }
    }
}