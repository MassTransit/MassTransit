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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;
    using InternalMessages;


    class FaultedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        readonly Activity _activity;
        readonly ActivityException _activityException;
        readonly TimeSpan _duration;
        readonly ExceptionInfo _exceptionInfo;
        readonly Execution<TArguments> _execution;
        readonly RoutingSlip _routingSlip;

        public FaultedExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            ExceptionInfo exceptionInfo)
        {
            _execution = execution;
            _activity = activity;
            _routingSlip = routingSlip;
            _exceptionInfo = exceptionInfo;
            _duration = _execution.Elapsed;

            _activityException = new ActivityExceptionImpl(_activity.Name, _execution.Host, _execution.ActivityTrackingNumber,
                _execution.Timestamp, _duration, _exceptionInfo);
        }

        public async Task Evaluate()
        {
            IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(_execution, _execution,_routingSlip);

            RoutingSlipActivityFaulted activityFaulted = new RoutingSlipActivityFaultedMessage(_execution.Host, _execution.TrackingNumber, _execution.ActivityName, _execution.ActivityTrackingNumber, _execution.Timestamp,
                _duration, _exceptionInfo, _routingSlip.Variables, _activity.Arguments);
            await publisher.Publish(activityFaulted);

            if (HasCompensationLogs())
            {
                RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

                Build(builder);

                RoutingSlip routingSlip = builder.Build();

                await _execution.ConsumeContext.Forward(routingSlip.GetNextCompensateAddress(), routingSlip);
            }
            else
            {
                DateTime faultedTimestamp = _execution.Timestamp + _duration;
                TimeSpan faultedDuration = faultedTimestamp - _routingSlip.CreateTimestamp;

                RoutingSlipFaulted routingSlipFaulted = new RoutingSlipFaultedMessage(_execution.TrackingNumber, faultedTimestamp,
                    faultedDuration, Enumerable.Repeat(_activityException, 1), _routingSlip.Variables);
                await publisher.Publish(routingSlipFaulted);
            }
        }

        bool HasCompensationLogs()
        {
            return _routingSlip.CompensateLogs != null && _routingSlip.CompensateLogs.Count > 0;
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityException(_activityException);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, (IEnumerable<Activity> x) => x);
        }
    }
}