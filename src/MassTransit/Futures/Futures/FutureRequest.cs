namespace MassTransit.Futures
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Middleware;
    using SagaStateMachine;
    using Transports;


    public class FutureRequest<TInput, TRequest> :
        ISpecification
        where TRequest : class
        where TInput : class
    {
        ContextMessageFactory<BehaviorContext<FutureState, TInput>, TRequest> _factory;

        public FutureRequest()
        {
            _factory = new ContextMessageFactory<BehaviorContext<FutureState, TInput>, TRequest>(DefaultFactory);

            AddressProvider = PublishAddressProvider;
        }

        public RequestAddressProvider<TInput> AddressProvider { get; set; }

        public PendingFutureIdProvider<TRequest> PendingRequestIdProvider { get; set; }

        public ContextMessageFactory<BehaviorContext<FutureState, TInput>, TRequest> Factory
        {
            set => _factory = value;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_factory == null)
                yield return this.Failure("Response", "Factory", "Init or Create must be configured");
            if (AddressProvider == null)
                yield return this.Failure("RequestAddressProvider", "must not be null");
        }

        static Uri PublishAddressProvider<T>(BehaviorContext<FutureState, T> context)
            where T : class
        {
            return default;
        }

        public async Task SendRequest(BehaviorContext<FutureState, TInput> context)
        {
            var destinationAddress = AddressProvider(context);

            var endpoint = destinationAddress != null
                ? await context.GetSendEndpoint(destinationAddress).ConfigureAwait(false)
                : new ConsumeSendEndpoint(await context.ReceiveContext.PublishEndpointProvider.GetPublishSendEndpoint<TRequest>().ConfigureAwait(false),
                    context, default);

            await _factory.Use(context, async (ctx, s) =>
            {
                var pipe = new FutureRequestPipe<TRequest>(s.Pipe, context.ReceiveContext.InputAddress, context.Saga.CorrelationId);

                await endpoint.Send(s.Message, pipe, context.CancellationToken).ConfigureAwait(false);

                if (PendingRequestIdProvider != null)
                {
                    var pendingId = PendingRequestIdProvider(s.Message);
                    context.Saga.Pending.Add(pendingId);
                }
            });
        }

        static Task<SendTuple<TRequest>> DefaultFactory(BehaviorContext<FutureState, TInput> context)
        {
            return context.Init<TRequest>(context.Message);
        }
    }
}
