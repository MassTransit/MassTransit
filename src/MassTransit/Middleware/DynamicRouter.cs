namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// A dynamic router is a pipe on which additional pipes can be connected and context is
    /// routed through the pipe based upon the output requirements of the connected pipes. It is built
    /// around the dynamic filter, which is the central point of the router.
    /// </summary>
    public class DynamicRouter<TContext> :
        IDynamicRouter<TContext>
        where TContext : class, PipeContext
    {
        readonly IDynamicFilter<TContext> _filter;
        readonly IPipe<TContext> _pipe;

        public DynamicRouter(IPipeContextConverterFactory<TContext> converterFactory)
        {
            _filter = new DynamicFilter<TContext>(converterFactory);
            _pipe = Pipe.New<TContext>(x => x.UseFilter(_filter));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("dynamicRouter");

            _pipe.Probe(scope);
        }

        Task IPipe<TContext>.Send(TContext context)
        {
            return _pipe.Send(context);
        }

        public ConnectHandle ConnectPipe<T>(IPipe<T> pipe)
            where T : class, PipeContext
        {
            return _filter.ConnectPipe(pipe);
        }

        ConnectHandle IFilterObserverConnector.ConnectObserver<T>(IFilterObserver<T> observer)
        {
            return _filter.ConnectObserver(observer);
        }

        ConnectHandle IFilterObserverConnector.ConnectObserver(IFilterObserver observer)
        {
            return _filter.ConnectObserver(observer);
        }
    }


    public class DynamicRouter<TContext, TKey> :
        IDynamicRouter<TContext, TKey>
        where TContext : class, PipeContext
    {
        readonly IDynamicFilter<TContext, TKey> _filter;
        readonly IPipe<TContext> _pipe;

        public DynamicRouter(IPipeContextConverterFactory<TContext> converterFactory, KeyAccessor<TContext, TKey> keyAccessor)
        {
            _filter = new DynamicFilter<TContext, TKey>(converterFactory, keyAccessor);
            _pipe = Pipe.New<TContext>(x => x.UseFilter(_filter));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("dynamicRouter");

            _pipe.Probe(scope);
        }

        Task IPipe<TContext>.Send(TContext context)
        {
            return _pipe.Send(context);
        }

        public ConnectHandle ConnectPipe<T>(IPipe<T> pipe)
            where T : class, PipeContext
        {
            return _filter.ConnectPipe(pipe);
        }

        ConnectHandle IFilterObserverConnector.ConnectObserver<T>(IFilterObserver<T> observer)
        {
            return _filter.ConnectObserver(observer);
        }

        ConnectHandle IFilterObserverConnector.ConnectObserver(IFilterObserver observer)
        {
            return _filter.ConnectObserver(observer);
        }

        public ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
            where T : class, PipeContext
        {
            return _filter.ConnectPipe(key, pipe);
        }
    }
}
