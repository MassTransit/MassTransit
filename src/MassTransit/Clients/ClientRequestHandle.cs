namespace MassTransit.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Util;


    public class ClientRequestHandle<TRequest> :
        RequestHandle<TRequest>,
        IPipe<SendContext<TRequest>>
        where TRequest : class
    {
        public delegate Task<TRequest> SendRequestCallback(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken);


        readonly IList<string> _accept;
        readonly CancellationToken _cancellationToken;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly ClientFactoryContext _context;
        readonly TaskCompletionSource<TRequest> _message;
        readonly IBuildPipeConfigurator<SendContext<TRequest>> _pipeConfigurator;
        readonly TaskCompletionSource<bool> _readyToSend;
        readonly Dictionary<Type, HandlerConnectHandle> _responseHandlers;
        readonly Task _send;
        readonly TaskCompletionSource<SendContext<TRequest>> _sendContext;
        readonly SendRequestCallback _sendRequestCallback;
        readonly TaskScheduler _taskScheduler;
        int _faultedOrCanceled;
        CancellationTokenRegistration _registration;
        Timer _timeoutTimer;
        RequestTimeout _timeToLive;

        public ClientRequestHandle(ClientFactoryContext context, SendRequestCallback sendRequestCallback, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default, Guid? requestId = default, TaskScheduler taskScheduler = default)
        {
            _context = context;
            _sendRequestCallback = sendRequestCallback;
            _cancellationToken = cancellationToken;

            var requestTimeout = timeout.HasValue ? timeout : _context.DefaultTimeout.HasValue ? _context.DefaultTimeout.Value : RequestTimeout.Default;
            _timeToLive = requestTimeout;

            RequestId = requestId ?? NewId.NextGuid();

            _taskScheduler = taskScheduler ??
                (SynchronizationContext.Current == null
                    ? TaskScheduler.Default
                    : TaskScheduler.FromCurrentSynchronizationContext());

            _message = new TaskCompletionSource<TRequest>();
            _pipeConfigurator = new PipeConfigurator<SendContext<TRequest>>();
            _sendContext = TaskUtil.GetTask<SendContext<TRequest>>();
            _readyToSend = TaskUtil.GetTask<bool>();
            _cancellationTokenSource = new CancellationTokenSource();
            _responseHandlers = new Dictionary<Type, HandlerConnectHandle>();
            _accept = new List<string>();

            if (cancellationToken != default && cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(Cancel);

            _timeoutTimer = new Timer(TimeoutExpired, this, (long)_timeToLive.Value.TotalMilliseconds, -1L);

            _send = SendRequest();

            HandleFault();
        }

        async Task IPipe<SendContext<TRequest>>.Send(SendContext<TRequest> context)
        {
            await _readyToSend.Task.ConfigureAwait(false);

            context.RequestId = ((RequestHandle)this).RequestId;
            context.ResponseAddress = _context.ResponseAddress;

            context.Headers.Set(MessageHeaders.Request.Accept, _accept);

            if (_timeToLive.HasValue)
                context.TimeToLive ??= _timeToLive.Value;

            IPipe<SendContext<TRequest>> pipe = _pipeConfigurator.Build();

            if (pipe.IsNotEmpty())
                await pipe.Send(context).ConfigureAwait(false);

            _sendContext.TrySetResult(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        public Guid RequestId { get; }

        public RequestTimeout TimeToLive
        {
            set => _timeToLive = value;
        }

        public void Cancel()
        {
            if (Interlocked.CompareExchange(ref _faultedOrCanceled, 1, 0) != 0)
                return;

            Task.Factory.StartNew(CancelAndDispose, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }

        void IPipeConfigurator<SendContext<TRequest>>.AddPipeSpecification(IPipeSpecification<SendContext<TRequest>> specification)
        {
            _pipeConfigurator.AddPipeSpecification(specification);
        }

        Task<Response<T>> RequestHandle.GetResponse<T>(bool readyToSend)
        {
            Task<Response<T>> response = Response<T>();

            AcceptResponse<T>();

            if (readyToSend)
                _readyToSend.TrySetResult(true);

            return response;
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _faultedOrCanceled, 1, 0) == 0)
                CancelAndDispose();
        }

        Task<TRequest> RequestHandle<TRequest>.Message => _message.Task;

        void AcceptResponse<T>()
            where T : class
        {
            _accept.Add(MessageUrn.ForTypeString<T>());
        }

        async Task SendRequest()
        {
            try
            {
                var message = await _sendRequestCallback(((RequestHandle)this).RequestId, this, _cancellationTokenSource.Token).ConfigureAwait(false);

                _message.TrySetResult(message);
            }
            catch (RequestException exception)
            {
                Fail(exception);

                throw;
            }
            catch (OperationCanceledException exception)
            {
                if (_sendContext.Task.IsFaulted)
                    await _sendContext.Task.ConfigureAwait(false);

                var requestException = new RequestCanceledException(((RequestHandle)this).RequestId.ToString("D"), exception, exception.CancellationToken);

                Fail(requestException);

                throw requestException;
            }
            catch (Exception exception)
            {
                Fail(exception);

                throw new RequestException($"An exception occurred while processing the {typeof(TRequest).Name} request", exception);
            }
        }

        Task<Response<T>> Response<T>(MessageHandler<T> handler = null, Action<IHandlerConfigurator<T>> configure = null)
            where T : class
        {
            if (_responseHandlers.ContainsKey(typeof(T)))
                throw new RequestException($"Only one handler of type {TypeCache<T>.ShortName} can be registered");

            var configurator = new ResponseHandlerConfigurator<T>(_taskScheduler, handler, _send);

            configure?.Invoke(configurator);

            if (_cancellationToken.IsCancellationRequested)
                return TaskUtil.Cancelled<Response<T>>();

            HandlerConnectHandle<T> handle = configurator.Connect(_context, ((RequestHandle)this).RequestId);

            _responseHandlers.Add(typeof(T), handle);

            return handle.Task;
        }

        void HandleFault()
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            Task MessageHandler(ConsumeContext<Fault<TRequest>> context)
            {
                return FaultHandler(context);
            }

            var connectHandle = _context.ConnectRequestHandler(((RequestHandle)this).RequestId, MessageHandler,
                new PipeConfigurator<ConsumeContext<Fault<TRequest>>>());

            var handle = new FaultHandlerConnectHandle(connectHandle);

            _responseHandlers.Add(typeof(Fault<TRequest>), handle);
        }

        Task FaultHandler(ConsumeContext<Fault<TRequest>> context)
        {
            Fail(context.Message);

            return Task.CompletedTask;
        }

        void Fail(Fault message)
        {
            Fail(new RequestFaultException(TypeCache<TRequest>.ShortName, message));
        }

        void Fail(Exception exception)
        {
            if (Interlocked.CompareExchange(ref _faultedOrCanceled, 1, 0) != 0)
                return;

            void HandleFail()
            {
                _registration.Dispose();

                DisposeTimer();

                _readyToSend.TrySetException(exception);

                var wasSet = _sendContext.TrySetException(exception);

                _message.TrySetException(exception);

                foreach (var handle in _responseHandlers.Values)
                {
                    handle.TrySetException(exception);
                    handle.Disconnect();
                }

                if (wasSet)
                    _cancellationTokenSource.Cancel();
            }

            Task.Factory.StartNew(HandleFail, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }

        void CancelAndDispose()
        {
            _registration.Dispose();

            DisposeTimer();

            _cancellationTokenSource.Cancel();

            var cancellationToken = _cancellationToken.IsCancellationRequested ? _cancellationToken : _cancellationTokenSource.Token;

            _readyToSend.TrySetCanceled(cancellationToken);

            _sendContext.TrySetCanceled(cancellationToken);

            _message.TrySetCanceled();

            foreach (var handle in _responseHandlers.Values)
            {
                handle.TrySetCanceled(cancellationToken);
                handle.Disconnect();
            }
        }

        void TimeoutExpired(object state)
        {
            var timeoutException = new RequestTimeoutException(((RequestHandle)this).RequestId.ToString());

            Fail(timeoutException);
        }

        void DisposeTimer()
        {
            try
            {
                _timeoutTimer?.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }

            _timeoutTimer = null;
        }
    }
}
