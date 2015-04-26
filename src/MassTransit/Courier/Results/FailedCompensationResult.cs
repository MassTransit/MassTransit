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
    using InternalMessages;


    class FailedCompensationResult<TLog> :
        CompensationResult
        where TLog : class
    {
        readonly CompensateLog _compensateLog;
        readonly Compensation<TLog> _compensation;
        readonly TimeSpan _duration;
        readonly Exception _exception;
        readonly RoutingSlip _routingSlip;

        public FailedCompensationResult(Compensation<TLog> compensation, CompensateLog compensateLog, RoutingSlip routingSlip,
            Exception exception)
        {
            _compensation = compensation;
            _compensateLog = compensateLog;
            _routingSlip = routingSlip;
            _exception = exception;
            _duration = _compensation.ElapsedTime;
        }

        public async Task Evaluate()
        {
            DateTime faultedTimestamp = _compensation.StartTimestamp + _duration;
            TimeSpan faultedDuration = faultedTimestamp - _routingSlip.CreateTimestamp;

            IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(_compensation, _compensation, _routingSlip);

            var activityCompensationFailed = new CompensationFailed(_compensation.Host,
                _compensation.TrackingNumber, _compensation.ActivityName, _compensation.ExecutionId, _compensation.StartTimestamp,
                _duration, faultedTimestamp, faultedDuration, _routingSlip.Variables, _compensateLog.Data, _exception);
            await publisher.Publish(activityCompensationFailed);
        }
    }
}