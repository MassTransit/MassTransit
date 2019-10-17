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

            await endpoint.Send<Fault<TRequest>>(
                    new FaultEvent<TRequest>(request, requestId, context.Host, exceptions.Select(x => x.ExceptionInfo),
                        context.SupportedMessageTypes.ToArray()), x => x.RequestId = requestId)
                .ConfigureAwait(false);
        }

        protected abstract TResponse CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, TRequest request);
    }
}
