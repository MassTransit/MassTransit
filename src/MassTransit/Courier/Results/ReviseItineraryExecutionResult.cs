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


    class ReviseItineraryExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Action<ItineraryBuilder> _itineraryBuilder;

        public ReviseItineraryExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            Action<ItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip)
        {
            _itineraryBuilder = itineraryBuilder;
        }

        public ReviseItineraryExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            IDictionary<string, object> data, Action<ItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip, data)
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
            return new RoutingSlipBuilder(routingSlip, Enumerable.Empty<Activity>(), routingSlip.Itinerary.Skip(1));
        }

        protected override async Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            await base.PublishActivityEvents(routingSlip, builder);

            await Publisher.PublishRoutingSlipRevised(Context.ExecutionId, Context.Timestamp, Context.Elapsed, routingSlip.Variables,
                routingSlip.Itinerary, builder.SourceItinerary);
        }
    }


    class ReviseItineraryExecutionResult<TArguments, TLog> :
        ReviseItineraryExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Uri _compensationAddress;

        public ReviseItineraryExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            Uri compensationAddress, TLog log, Action<ItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip, RoutingSlipBuilder.GetObjectAsDictionary(log), itineraryBuilder)
        {
            _compensationAddress = compensationAddress;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            builder.AddCompensateLog(Context.ExecutionId, _compensationAddress, Data);

            base.Build(builder);
        }

    }
}