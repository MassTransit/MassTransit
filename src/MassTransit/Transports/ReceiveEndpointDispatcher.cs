namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Courier;
    using Internals;


    public class ReceiveEndpointDispatcher :
        IReceiveEndpointDispatcher
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        public ReceiveEndpointDispatcher(ReceiveEndpointContext context)
        {
            _context = context;

            _dispatcher = context.CreateReceivePipeDispatcher();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("dispatcher");

            _context.ReceivePipe.Probe(scope);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }

        public int ActiveDispatchCount => _dispatcher.ActiveDispatchCount;

        public long DispatchCount => _dispatcher.DispatchCount;

        public int MaxConcurrentDispatchCount => _dispatcher.MaxConcurrentDispatchCount;

        public event ZeroActiveDispatchHandler ZeroActivity
        {
            add => _dispatcher.ZeroActivity += value;
            remove => _dispatcher.ZeroActivity -= value;
        }

        public DeliveryMetrics GetMetrics()
        {
            return _dispatcher.GetMetrics();
        }

        public Uri InputAddress => _context.InputAddress;

        public async Task Dispatch(byte[] body, IReadOnlyDictionary<string, object> headers, CancellationToken cancellationToken, params object[] payloads)
        {
            var context = new ReceiveEndpointDispatcherReceiveContext(_context, body, headers, payloads);

            CancellationTokenRegistration registration = default;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(context.Cancel);

            try
            {
                await _dispatcher.Dispatch(context).ConfigureAwait(false);
            }
            finally
            {
                registration.Dispose();
                context.Dispose();
            }
        }
    }


    public class ReceiveEndpointDispatcher<T> :
        IReceiveEndpointDispatcher<T>
        where T : class
    {
        readonly IReceiveEndpointDispatcher _dispatcher;

        public ReceiveEndpointDispatcher(IReceiveEndpointDispatcherFactory factory)
            : this(factory, DefaultEndpointNameFormatter.Instance)
        {
        }

        public ReceiveEndpointDispatcher(IReceiveEndpointDispatcherFactory factory, IEndpointNameFormatter formatter)
        {
            IReceiveEndpointDispatcher CreateDispatcher(Type dispatcherType)
            {
                var dispatcherFactory = (ITypeReceiveEndpointDispatcherFactory)Activator.CreateInstance(dispatcherType);

                return dispatcherFactory.Create(factory, formatter);
            }

            if (typeof(T).HasInterface<ISaga>())
                _dispatcher = CreateDispatcher(typeof(SagaReceiveEndpointDispatcher<>).MakeGenericType(typeof(T)));
            else if (typeof(T).HasInterface<IConsumer>())
                _dispatcher = CreateDispatcher(typeof(ConsumerReceiveEndpointDispatcher<>).MakeGenericType(typeof(T)));
            else if (typeof(T).ClosesType(typeof(IExecuteActivity<>), out Type[] arguments))
                _dispatcher = CreateDispatcher(typeof(ExecuteActivityReceiveEndpointDispatcher<,>).MakeGenericType(typeof(T), arguments[0]));
            else
                _dispatcher = factory.CreateReceiver(formatter.Message<T>());
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _dispatcher.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T1>(IConsumeMessageObserver<T1> observer)
            where T1 : class
        {
            return _dispatcher.ConnectConsumeMessageObserver(observer);
        }

        public int ActiveDispatchCount => _dispatcher.ActiveDispatchCount;
        public long DispatchCount => _dispatcher.DispatchCount;
        public int MaxConcurrentDispatchCount => _dispatcher.MaxConcurrentDispatchCount;

        public event ZeroActiveDispatchHandler ZeroActivity
        {
            add => _dispatcher.ZeroActivity += value;
            remove => _dispatcher.ZeroActivity -= value;
        }

        public DeliveryMetrics GetMetrics()
        {
            return _dispatcher.GetMetrics();
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _dispatcher.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _dispatcher.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _dispatcher.ConnectSendObserver(observer);
        }

        public void Probe(ProbeContext context)
        {
            _dispatcher.Probe(context);
        }

        public Uri InputAddress => _dispatcher.InputAddress;

        public Task Dispatch(byte[] body, IReadOnlyDictionary<string, object> headers, CancellationToken cancellationToken, params object[] payloads)
        {
            return _dispatcher.Dispatch(body, headers, cancellationToken, payloads);
        }
    }
}
