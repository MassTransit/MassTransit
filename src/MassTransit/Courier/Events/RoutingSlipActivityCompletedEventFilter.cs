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
namespace MassTransit.Courier.Events
{
    using System.Collections.Generic;
    using Contracts;
    using InternalMessages;
    using Newtonsoft.Json;


    public class RoutingSlipActivityCompletedEventFilter :
        IRoutingSlipEventFilter
    {
        T IRoutingSlipEventFilter.Filter<T>(T message, RoutingSlipEventContents contents)
        {
            var source = message as RoutingSlipActivityCompleted;
            if (source == null)
                return message;

            RoutingSlipActivityCompleted result = new RoutingSlipActivityCompletedMessage(
                source.Host,
                source.TrackingNumber,
                source.ActivityName,
                source.ActivityTrackingNumber,
                source.Timestamp,
                source.Duration,
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Variables))
                    ? source.Variables
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Arguments))
                    ? source.Arguments
                    : GetEmptyObject(),
                (contents == RoutingSlipEventContents.All || contents.HasFlag(RoutingSlipEventContents.Data))
                    ? source.Data
                    : GetEmptyObject());

            return result as T;
        }

        static IDictionary<string, object> GetEmptyObject()
        {
            return JsonConvert.DeserializeObject<IDictionary<string, object>>("{}");
        }
    }
}