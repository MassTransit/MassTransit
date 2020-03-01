namespace MassTransit.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using Metadata;
    using Util;


    public class ClientRequestHandle<TRequest> :
        RequestHandle<TRequest>,
        IPipe<SendContext<TRequest>>
        where TRequest : class
    {
        public delegate Task<TRequest> SendRequestCallback(Guid requestId, IPipe<SendContext<TRequest>> pipe, CancellationToken cancellationToken);


        readonly CancellationToken _cancellationToken;
        readonly ClientFactoryContext _context;
        readonly SendRequestCallback _sendRequestCallback;
        readonly TaskCompletionSource<TRequest> _message;
        readonly IBuildPipeConfigurator<SendContext<TRequest>> _pipeConfigurator;
        readonly TaskCompletionSource<bool> _readyToSend;
        readonly Guid _requestId;
        readonly Dictionary<Type, HandlerConnectHandle> _responseHandlers;
        readonly Task _send;
        readonly TaskCompletionSource<SendContext<TRequest>> _sendContext;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly TaskScheduler _taskScheduler;
        CancellationTokenRegistration _registration;
        Timer _timeoutTimer;
        int _faultedOrCanceled;
        RequestTimeout _timeToLive;

        public ClientRequestHandle(ClientFactoryContext context, SendRequestCallback sendRequestCallback, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default, Guid? requestId = default, TaskScheduler taskScheduler = default)
        {
            _context = context;
            _sendRequestCallback = sendRequestCallback;
            _cancellationToken = cancellationToken;

            var requestTimeout = timeout.HasValue ? timeout : _context.DefaultTimeout.HasValue ? _context.DefaultTimeout.Value : RequestTimeout.Default;
            _timeToLive = requestTimeout;

            _requestId = requestId ?? NewId.NextGuid();

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

            if (cancellationToken != default && cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(Cancel);

            _timeoutTimer = new Timer(TimeoutExpired, this, (long)_timeToLive.Value.TotalMilliseconds, -1L);

            _send = SendRequest();

        #pragma warning disable 4014
            HandleFault();
        #pragma warning restore 4014
        }

        async Task IPipe<SendContext<TRequest>>.Send(SendContext<TRequest> context)
        {
            await _readyToSend.Task.ConfigureAwait(false);

            context.RequestId = _requestId;
            context.ResponseAddress = _context.ResponseAddress;

            if (_timeToLive.HasValue)
                context.TimeToLive = _timeToLive.Value;

            IPipe<SendContext<TRequest>> pipe = _pipeConfigurator.Build();

            if (pipe.IsNotEmpty())
                await pipe.Send(context).ConfigureAwait(false);

            _sendContext.TrySetResult(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        Guid RequestHandle.RequestId => _requestId;

        RequestTimeout RequestHandle.TimeToLive
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

        async Task SendRequest()
        {
            try
            {
                var message = await _sendRequestCallback(_requestId, this, _cancellationTokenSource.Token).ConfigureAwait(false);

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

                var requestException = new RequestCanceledException(_requestId.ToString("D"), exception, exception.CancellationToken);

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
                throw new RequestException($"Only one handler of type {TypeMetadataCache<T>.ShortName} can be registered");

            var configurator = new ResponseHandlerConfigurator<T>(_taskScheduler, handler, _send);

            configure?.Invoke(configurator);

            if (_cancellationToken.IsCancellationRequested)
                return TaskUtil.Cancelled<Response<T>>();

            HandlerConnectHandle<T> handle = configurator.Connect(_context, _requestId);

            _responseHandlers.Add(typeof(T), handle);

            return handle.Task;
        }

        async Task HandleFault()
        {
            try
            {
                await Response<Fault<TRequest>>(FaultHandler).ConfigureAwait(false);
            }
            catch
            {
                // need to observe the exception, if it is faulted
            }
        }

        Task FaultHandler(ConsumeContext<Fault<TRequest>> context)
        {
            Fail(context.Message);

            return TaskUtil.Completed;
        }

        void Fail(Fault message)
        {
            Fail(new RequestFaultException(TypeMetadataCache<TRequest>.ShortName, message));
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

                bool cancel = _sendContext.TrySetException(exception);
                if (cancel)
                    _cancellationTokenSource.Cancel();

                _message.TrySetException(exception);

                foreach (var handle in _responseHandlers.Values)
                {
                    handle.TrySetException(exception);
                    handle.Disconnect();
                }
            }

            Task.Factory.StartNew(HandleFail, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }

        void CancelAndDispose()
        {
            _registration.Dispose();

            DisposeTimer();

            _readyToSend.TrySetCanceled();

            bool cancel = _sendContext.TrySetCanceled();
            if (cancel)
                _cancellationTokenSource.Cancel();

            _message.TrySetCanceled();

            foreach (var handle in _responseHandlers.Values)
            {
                handle.TrySetCanceled();
                handle.Disconnect();
            }
        }

        void TimeoutExpired(object state)
        {
            var timeoutException = new RequestTimeoutException(_requestId.ToString());

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
