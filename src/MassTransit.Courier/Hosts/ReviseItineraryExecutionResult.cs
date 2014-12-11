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
    using Contracts;


    class ReviseItineraryExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Action<ItineraryBuilder> _itineraryBuilder;

        public ReviseItineraryExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            Action<ItineraryBuilder> itineraryBuilder)
            : base(execution, activity, routingSlip)
        {
            _itineraryBuilder = itineraryBuilder;
        }

        public ReviseItineraryExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            IDictionary<string, object> data, Action<ItineraryBuilder> itineraryBuilder)
            : base(execution, activity, routingSlip, data)
        {
            _itineraryBuilder = itineraryBuilder;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            _itineraryBuilder(builder);
        }

        protected override RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            var builder = new RoutingSlipBuilder(routingSlip, x => Enumerable.Empty<Activity>());

            builder.SetSourceItinerary(routingSlip.Itinerary.Skip(1));

            return builder;
        }
    }


    class ReviseItineraryExecutionResult<TArguments, TLog> :
        ReviseItineraryExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Uri _compensationAddress;

        public ReviseItineraryExecutionResult(Execution<TArguments> execution, Activity activity, RoutingSlip routingSlip,
            Uri compensationAddress, TLog log, Action<ItineraryBuilder> itineraryBuilder)
            : base(execution, activity, routingSlip, RoutingSlipBuilder.GetObjectAsDictionary(log), itineraryBuilder)
        {
            _compensationAddress = compensationAddress;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            builder.AddCompensateLog(Execution.ActivityTrackingNumber, _compensationAddress, Data);

            base.Build(builder);
        }

        protected override RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            var builder = new RoutingSlipBuilder(routingSlip, x => Enumerable.Empty<Activity>());

            builder.SetSourceItinerary(routingSlip.Itinerary.Skip(1));

            return builder;
        }
    }
}