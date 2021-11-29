namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;


    public abstract class RoutingSlipResponseProxy<TRequest, TResponse, TFaultResponse> :
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipFaulted>
        where TRequest : class
        where TResponse : class
        where TFaultResponse : class
    {
        public virtual async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            var request = context.GetVariable<TRequest>("Request");

            Guid? requestId = context.GetVariable<Guid>("RequestId")
                ?? throw new ArgumentException($"The RequestId variable was not found on the completed routing slip: {context.Message.TrackingNumber}");

            var responseAddress = context.GetVariable<Uri>("ResponseAddress")
                ?? throw new ArgumentException($"The ResponseAddress variable was not found on the completed routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetResponseEndpoint<TResponse>(responseAddress, requestId).ConfigureAwait(false);

            var response = await CreateResponseMessage(context, request);

            await endpoint.Send(response).ConfigureAwait(false);
        }

        public virtual async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            var request = context.GetVariable<TRequest>("Request");

            Guid? requestId = context.GetVariable<Guid>("RequestId")
                ?? throw new ArgumentException($"The RequestId variable was not found on the faulted routing slip: {context.Message.TrackingNumber}");

            var faultAddress = context.GetVariable<Uri>("FaultAddress") ?? context.GetVariable<Uri>("ResponseAddress")
                ?? throw new ArgumentException($"The (Fault|Response)Address was not found on the faulted routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetFaultEndpoint<TResponse>(faultAddress, requestId).ConfigureAwait(false);

            var response = await CreateFaultedResponseMessage(context, request, requestId.Value);

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

            Fault<TRequest> response = new FaultEvent<TRequest>(request, requestId, context.Host, exceptions, MessageTypeCache<TRequest>.MessageTypeNames);

            return Task.FromResult(response);
        }
    }
}
