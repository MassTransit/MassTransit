namespace MassTransit
{
    using System;


    public interface IBindConfigurator<TLeft>
        where TLeft : class, PipeContext
    {
        /// <summary>
        /// Specifies a pipe context source which is used to create the PipeContext bound to the BindContext.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="configureTarget"></param>
        /// <typeparam name="T"></typeparam>
        void Source<T>(IPipeContextSource<T, TLeft> source, Action<IBindConfigurator<TLeft, T>> configureTarget)
            where T : class, PipeContext;
    }


    /// <summary>
    /// Configures a binding using the specified pipe context source
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public interface IBindConfigurator<TLeft, TRight> :
        IPipeConfigurator<BindContext<TLeft, TRight>>
        where TRight : class, PipeContext
        where TLeft : class, PipeContext
    {
        /// <summary>
        /// Configure a filter on the context pipe, versus the bound pipe
        /// </summary>
        IPipeConfigurator<TLeft> ContextPipe { get; }
    }
}
