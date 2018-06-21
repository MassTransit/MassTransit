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
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;


    public abstract class RoutingSlipResponseProxy<TRequest, TResponse> :
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipFaulted>
        where TRequest : class
        where TResponse : class
    {
        public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            var request = context.Message.GetVariable<TRequest>("Request");
            var requestId = context.Message.GetVariable<Guid>("RequestId");

            Uri responseAddress = null;
            if (context.Message.Variables.ContainsKey("FaultAddress"))
                responseAddress = context.Message.GetVariable<Uri>("FaultAddress");
            if (responseAddress == null && context.Message.Variables.ContainsKey("ResponseAddress"))
                responseAddress = context.Message.GetVariable<Uri>("ResponseAddress");

            if (responseAddress == null)
                throw new ArgumentException($"The response address could not be found for the faulted routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetSendEndpoint(responseAddress).ConfigureAwait(false);

            var response = CreateResponseMessage(context, request);

            await endpoint.Send(response, x => x.RequestId = requestId)
                .ConfigureAwait(false);
        }

        public async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            var request = context.Message.GetVariable<TRequest>("Request");
            var requestId = context.Message.GetVariable<Guid>("RequestId");

            Uri responseAddress = null;
            if (context.Message.Variables.ContainsKey("FaultAddress"))
                responseAddress = context.Message.GetVariable<Uri>("FaultAddress");
            if (responseAddress == null && context.Message.Variables.ContainsKey("ResponseAddress"))
                responseAddress = context.Message.GetVariable<Uri>("ResponseAddress");

            if (responseAddress == null)
                throw new ArgumentException($"The response address could not be found for the faulted routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetSendEndpoint(responseAddress).ConfigureAwait(false);

            ActivityException[] exceptions = context.Message.ActivityExceptions;

            await endpoint.Send<Fault<TRequest>>(new FaultEvent<TRequest>(request, requestId, context.Host, exceptions.Select(x => x.ExceptionInfo)),
                x => x.RequestId = requestId)
                .ConfigureAwait(false);
        }

        protected abstract TResponse CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, TRequest request);
    }
}