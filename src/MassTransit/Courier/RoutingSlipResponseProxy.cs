namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;
    using Metadata;


    public abstract class RoutingSlipResponseProxy<TRequest, TResponse, TFaultResponse> :
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipFaulted>
        where TRequest : class
        where TResponse : class
        where TFaultResponse : class
    {
        public async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            var request = context.Message.GetVariable<TRequest>("Request");
            var requestId = context.Message.GetVariable<Guid>("RequestId");

            Uri responseAddress = null;
            if (context.Message.Variables.ContainsKey("ResponseAddress"))
                responseAddress = context.Message.GetVariable<Uri>("ResponseAddress");

            if (responseAddress == null)
                throw new ArgumentException($"The response address could not be found for the faulted routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetResponseEndpoint<TResponse>(responseAddress, requestId).ConfigureAwait(false);

            var response = await CreateResponseMessage(context, request);

            await endpoint.Send(response).ConfigureAwait(false);
        }

        public async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            var request = context.Message.GetVariable<TRequest>("Request");
            var requestId = context.Message.GetVariable<Guid>("RequestId");

            Uri faultAddress = null;
            if (context.Message.Variables.ContainsKey("FaultAddress"))
                faultAddress = context.Message.GetVariable<Uri>("FaultAddress");
            if (faultAddress == null && context.Message.Variables.ContainsKey("ResponseAddress"))
                faultAddress = context.Message.GetVariable<Uri>("ResponseAddress");

            if (faultAddress == null)
                throw new ArgumentException($"The fault/response address could not be found for the faulted routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetFaultEndpoint<TResponse>(faultAddress, requestId).ConfigureAwait(false);

            var response = await CreateFaultedResponseMessage(context, request, requestId);

            await endpoint.Send(response).ConfigureAwait(false);
        }

        protected abstract Task<TResponse> CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, TRequest request);

        protected abstract Task<TFaultResponse> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipFaulted> context, TRequest request, Guid requestId);
    }


    public abstract class RoutingSlipResponseProxy<TRequest, TResponse> :
        RoutingSlipResponseProxy<TRequest, TResponse, Fault<TRequest>>
        where TRequest : class
        where TResponse : class
    {
        protected override Task<Fault<TRequest>> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipFaulted> context, TRequest request, Guid requestId)
        {
            IEnumerable<ExceptionInfo> exceptions = context.Message.ActivityExceptions.Select(x => x.ExceptionInfo);

            Fault<TRequest> response = new FaultEvent<TRequest>(request, requestId, context.Host, exceptions, TypeMetadataCache<TRequest>.MessageTypeNames);

            return Task.FromResult(response);
        }
    }
}
