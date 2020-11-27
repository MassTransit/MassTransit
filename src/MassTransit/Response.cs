namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;


    /// <summary>
    /// The base response type, which can be used to pattern match, via deconstruct, to the accepted
    /// response types.
    /// </summary>
    public interface Response :
        MessageContext
    {
        object Message { get; }
    }


    /// <summary>
    /// The response for a request with a single response type, or a request with multiple response types
    /// that has been matched to a specific type.
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    public interface Response<out TResponse> :
        Response
        where TResponse : class
    {
        /// <summary>
        /// The response message that was received
        /// </summary>
        new TResponse Message { get; }
    }


    /// <summary>
    /// The response for a request that accepts two response types, which can be matched easily or converted back into a tuple of
    /// tasks.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public readonly struct Response<T1, T2> :
        Response
        where T1 : class
        where T2 : class
    {
        readonly Response<T1> _response1;
        readonly Response<T2> _response2;
        readonly Response _response;
        readonly Task<Response<T1>> _response1Task;
        readonly Task<Response<T2>> _response2Task;

        public Response(Task<Response<T1>> response1, Task<Response<T2>> response2)
        {
            _response1Task = response1;
            _response2Task = response2;

            _response1 = response1.IsCompletedSuccessfully() ? response1.GetAwaiter().GetResult() : default;
            _response2 = response2.IsCompletedSuccessfully() ? response2.GetAwaiter().GetResult() : default;

            _response = (Response)_response1 ?? _response2 ?? throw new ArgumentException("At least one of the responses must be completed", nameof(response1));
        }

        public bool Is(out Response<T1> result)
        {
            result = _response1;

            return result != default;
        }

        public bool Is(out Response<T2> result)
        {
            result = _response2;

            return result != default;
        }

        public bool Is<T>(out Response<T> result)
            where T : class
        {
            if (_response1 is Response<T> response1)
            {
                result = response1;
                return true;
            }

            if (_response2 is Response<T> response2)
            {
                result = response2;
                return true;
            }

            result = default;
            return false;
        }

        public void Deconstruct(out Task<Response<T1>> r1, out Task<Response<T2>> r2)
        {
            r1 = _response1Task;
            r2 = _response2Task;
        }

        public static implicit operator Response<T1, T2>((Task<Response<T1>> response1, Task<Response<T2>> response2) source)
        {
            return new Response<T1, T2>(source.response1, source.response2);
        }

        public Guid? MessageId => _response.MessageId;

        public Guid? RequestId => _response.RequestId;

        public Guid? CorrelationId => _response.CorrelationId;

        public Guid? ConversationId => _response.ConversationId;

        public Guid? InitiatorId => _response.InitiatorId;

        public DateTime? ExpirationTime => _response.ExpirationTime;

        public Uri SourceAddress => _response.SourceAddress;

        public Uri DestinationAddress => _response.DestinationAddress;

        public Uri ResponseAddress => _response.ResponseAddress;

        public Uri FaultAddress => _response.FaultAddress;

        public DateTime? SentTime => _response.SentTime;

        public Headers Headers => _response.Headers;

        public HostInfo Host => _response.Host;

        public object Message => _response.Message;
    }
}
