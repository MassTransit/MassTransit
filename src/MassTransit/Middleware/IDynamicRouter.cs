namespace MassTransit.Middleware
{
    /// <summary>
    /// A dynamic router is a pipe on which additional pipes can be connected and context is
    /// routed through the pipe based upon the output requirements of the connected pipes. It is built
    /// around the dynamic filter, which is the central point of the router.
    /// </summary>
    public interface IDynamicRouter<in TContext> :
        IPipe<TContext>,
        IPipeConnector,
        IFilterObserverConnector
        where TContext : class, PipeContext
    {
    }


    /// <summary>
    /// A dynamic router is a pipe on which additional pipes can be connected and context is
    /// routed through the pipe based upon the output requirements of the connected pipes. It is built
    /// around the dynamic filter, which is the central point of the router.
    /// </summary>
    public interface IDynamicRouter<in TContext, in TKey> :
        IDynamicRouter<TContext>,
        IKeyPipeConnector<TKey>
        where TContext : class, PipeContext
    {
    }
}
