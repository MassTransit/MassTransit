namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Metadata;
    using MassTransit.Util;


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

        protected async Task SendRequest(BehaviorContext<TInstance> context, ConsumeContext consumeContext, TRequest requestMessage, Uri serviceAddress)
        {
            var pipe = new SendRequestPipe(consumeContext.ReceiveContext.InputAddress);

            var endpoint = await consumeContext.GetSendEndpoint(serviceAddress).ConfigureAwait(false);

            await endpoint.Send(requestMessage, pipe).ConfigureAwait(false);

            _request.SetRequestId(context.Instance, pipe.RequestId);

            if (_request.Settings.Timeout > TimeSpan.Zero)
            {
                var now = DateTime.UtcNow;
                var expirationTime = now + _request.Settings.Timeout;

                RequestTimeoutExpired<TRequest> message = new TimeoutExpired<TRequest>(now, expirationTime, context.Instance.CorrelationId, pipe.RequestId);

                if (consumeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                    await schedulerContext.ScheduleSend(expirationTime, message).ConfigureAwait(false);
                else
                    throw new ConfigurationException("A request timeout was specified but no message scheduler was specified or available");
            }
        }

        public virtual void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("request");
            scope.Add("requestType", TypeMetadataCache<TRequest>.ShortName);
            scope.Add("responseType", TypeMetadataCache<TResponse>.ShortName);
            scope.Set(_request.Settings);
        }


        /// <summary>
        /// Handles the sending of a request to the endpoint specified
        /// </summary>
        class SendRequestPipe :
            IPipe<SendContext<TRequest>>
        {
            readonly Guid _requestId;
            readonly Uri _responseAddress;

            public SendRequestPipe(Uri responseAddress)
            {
                _responseAddress = responseAddress;
                _requestId = NewId.NextGuid();
            }

            public Guid RequestId => _requestId;

            void IProbeSite.Probe(ProbeContext context)
            {
            }

            Task IPipe<SendContext<TRequest>>.Send(SendContext<TRequest> context)
            {
                context.RequestId = _requestId;
                context.ResponseAddress = _responseAddress;

                return TaskUtil.Completed;
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
