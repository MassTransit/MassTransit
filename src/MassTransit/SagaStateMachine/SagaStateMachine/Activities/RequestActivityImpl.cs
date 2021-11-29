namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Transports;


    public abstract class RequestActivityImpl<TInstance, TRequest, TResponse>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly Request<TInstance, TRequest, TResponse> _request;

        protected RequestActivityImpl(Request<TInstance, TRequest, TResponse> request)
        {
            _request = request;
        }

        protected async Task SendRequest(BehaviorContext<TInstance> context, SendTuple<TRequest> sendTuple, Uri serviceAddress)
        {
            var requestId = _request.GenerateRequestId(context.Saga);

            var pipe = new SendRequestPipe(_request, context.ReceiveContext.InputAddress, requestId, sendTuple.Pipe);

            var endpoint = serviceAddress == null
                ? new ConsumeSendEndpoint(await context.ReceiveContext.PublishEndpointProvider.GetPublishSendEndpoint<TRequest>().ConfigureAwait
                    (false), context, default)
                : await context.GetSendEndpoint(serviceAddress).ConfigureAwait(false);

            await endpoint.Send(sendTuple.Message, pipe).ConfigureAwait(false);

            _request.SetRequestId(context.Saga, requestId);

            if (_request.Settings.Timeout > TimeSpan.Zero)
            {
                var now = DateTime.UtcNow;
                var expirationTime = now + _request.Settings.Timeout;

                RequestTimeoutExpired<TRequest> message = new TimeoutExpired<TRequest>(now, expirationTime, context.Saga.CorrelationId, pipe.RequestId);

                if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
                    await schedulerContext.ScheduleSend(expirationTime, message).ConfigureAwait(false);
                else
                    throw new ConfigurationException("A request timeout was specified but no message scheduler was specified or available");
            }
        }

        public virtual void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("request");
            scope.Add("requestType", TypeCache<TRequest>.ShortName);
            scope.Add("responseType", TypeCache<TResponse>.ShortName);
            scope.Set(_request.Settings);
        }


        /// <summary>
        /// Handles the sending of a request to the endpoint specified
        /// </summary>
        class SendRequestPipe :
            IPipe<SendContext<TRequest>>
        {
            readonly Request<TInstance, TRequest, TResponse> _request;
            readonly Uri _responseAddress;
            readonly IPipe<SendContext<TRequest>> _pipe;

            public SendRequestPipe(Request<TInstance, TRequest, TResponse> request, Uri responseAddress, Guid requestId, IPipe<SendContext<TRequest>> pipe)
            {
                _request = request;
                _responseAddress = responseAddress;

                RequestId = requestId;

                if (pipe.IsNotEmpty())
                    _pipe = pipe;
            }

            public Guid RequestId { get; }

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            Task IPipe<SendContext<TRequest>>.Send(SendContext<TRequest> context)
            {
                context.RequestId = RequestId;
                context.ResponseAddress = _responseAddress;

                _request.SetSendContextHeaders(context);

                return _pipe != null
                    ? _pipe.Send(context)
                    : Task.CompletedTask;
            }
        }


        class TimeoutExpired<T> :
            RequestTimeoutExpired<T>
            where T : class
        {
            public TimeoutExpired(DateTime timestamp, DateTime expirationTime, Guid correlationId, Guid requestId)
            {
                Timestamp = timestamp;
                ExpirationTime = expirationTime;
                CorrelationId = correlationId;
                RequestId = requestId;
            }

            public DateTime Timestamp { get; }

            public DateTime ExpirationTime { get; }

            public Guid CorrelationId { get; }

            public Guid RequestId { get; }
        }
    }
}
