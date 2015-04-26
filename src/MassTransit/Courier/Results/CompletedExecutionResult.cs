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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;
    using InternalMessages;


    abstract class CompletedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        readonly Activity _activity;
        readonly IDictionary<string, object> _data;
        readonly TimeSpan _duration;
        readonly Execution<TArguments> _execution;
        readonly RoutingSlip _routingSlip;

        protected CompletedExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip)
            : this(execution, activity, routingSlip, RoutingSlipBuilder.NoArguments)
        {
        }

        protected CompletedExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            IDictionary<string, object> data)
        {
            _execution = execution;
            _activity = activity;
            _routingSlip = routingSlip;
            _data = data;
            _duration = _execution.Elapsed;
        }

        protected IDictionary<string, object> Data
        {
            get { return _data; }
        }

        protected Execution<TArguments> Execution
        {
            get { return _execution; }
        }

        protected Activity Activity
        {
            get { return _activity; }
        }

        protected TimeSpan Duration
        {
            get { return _duration; }
        }

        public async Task Evaluate()
        {
            RoutingSlipBuilder builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            RoutingSlip routingSlip = builder.Build();

            IRoutingSlipEventPublisher publisher = new RoutingSlipEventPublisher(_execution, _execution, routingSlip);

            RoutingSlipActivityCompleted message = new RoutingSlipActivityCompletedMessage(_execution.Host, _execution.TrackingNumber,
                _execution.ActivityName, _execution.ExecutionId, _execution.Timestamp, _duration,
                routingSlip.Variables, _activity.Arguments, _data);
            await publisher.Publish(message);

            if (HasNextActivity(routingSlip))
            {
                ISendEndpoint endpoint = await _execution.GetSendEndpoint(routingSlip.GetNextExecuteAddress());
                await _execution.ConsumeContext.Forward(endpoint, routingSlip);
            }
            else
            {
                DateTime completedTimestamp = _execution.Timestamp + _duration;
                TimeSpan completedDuration = completedTimestamp - _routingSlip.CreateTimestamp;

                RoutingSlipCompleted completedEvent = new RoutingSlipCompletedMessage(_routingSlip.TrackingNumber, completedTimestamp,
                    completedDuration, routingSlip.Variables);
                await publisher.Publish(completedEvent);
            }
        }

        static bool HasNextActivity(RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Any();
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityLog(Execution.Host, Activity.Name, Execution.ExecutionId, Execution.Timestamp, Duration);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, (IEnumerable<Activity> x) => x.Skip(1));
        }
    }
}