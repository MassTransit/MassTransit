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
    using System.Threading.Tasks;
    using Contracts;
    using Events;


    class FailedCompensationResult<TLog> :
        CompensationResult
        where TLog : class
    {
        readonly CompensateLog _compensateLog;
        readonly CompensateContext<TLog> _compensateContext;
        readonly TimeSpan _duration;
        readonly Exception _exception;
        readonly IRoutingSlipEventPublisher _publisher;
        readonly RoutingSlip _routingSlip;

        public FailedCompensationResult(CompensateContext<TLog> compensateContext, IRoutingSlipEventPublisher publisher, CompensateLog compensateLog,
            RoutingSlip routingSlip,
            Exception exception)
        {
            _compensateContext = compensateContext;
            _publisher = publisher;
            _compensateLog = compensateLog;
            _routingSlip = routingSlip;
            _exception = exception;
            _duration = _compensateContext.ElapsedTime;
        }

        public Task Evaluate()
        {
            DateTime faultedTimestamp = _compensateContext.StartTimestamp + _duration;
            TimeSpan faultedDuration = faultedTimestamp - _routingSlip.CreateTimestamp;

            return _publisher.PublishRoutingSlipActivityCompensationFailed(_compensateContext.ActivityName, _compensateContext.ExecutionId,
                _compensateContext.StartTimestamp, _duration, faultedTimestamp, faultedDuration, new FaultExceptionInfo(_exception), _routingSlip.Variables,
                _compensateLog.Data);
        }
    }
}