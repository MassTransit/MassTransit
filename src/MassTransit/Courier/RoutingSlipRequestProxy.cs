// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Contracts;


    public abstract class RoutingSlipRequestProxy<TRequest> :
        IConsumer<TRequest>
        where TRequest : class
    {
        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddSubscription(context.ReceiveContext.InputAddress, RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddVariable("RequestId", context.RequestId);
            builder.AddVariable("ResponseAddress", context.ResponseAddress);
            builder.AddVariable("FaultAddress", context.FaultAddress);
            builder.AddVariable("Request", context.Message);

            await BuildRoutingSlip(builder, context);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip).ConfigureAwait(false);
        }

        protected abstract Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<TRequest> request);
    }
}
