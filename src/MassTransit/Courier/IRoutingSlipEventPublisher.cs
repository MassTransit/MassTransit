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
namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;


    public interface IRoutingSlipEventPublisher
    {
        Task PublishRoutingSlipCompleted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables);

        Task PublishRoutingSlipFaulted(DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            params ActivityException[] exceptions);

        Task PublishRoutingSlipActivityCompleted(string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IDictionary<string, object> arguments,
            IDictionary<string, object> data);

        Task PublishRoutingSlipActivityFaulted(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, ExceptionInfo exceptionInfo,
            IDictionary<string, object> variables, IDictionary<string, object> arguments);

        Task PublishRoutingSlipActivityCompensationFailed(string activityName, Guid executionId,
            DateTime timestamp, TimeSpan duration, DateTime failureTimestamp, TimeSpan routingSlipDuration,
            ExceptionInfo exceptionInfo, IDictionary<string, object> variables, IDictionary<string, object> data);

        Task PublishRoutingSlipActivityCompensated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables, IDictionary<string, object> data);

        Task PublishRoutingSlipRevised(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables, IList<Activity> itinerary,
            IList<Activity> previousItinerary);

        Task PublishRoutingSlipTerminated(string activityName, Guid executionId, DateTime timestamp, TimeSpan duration, IDictionary<string, object> variables,
            IList<Activity> previousItinerary);
    }
}