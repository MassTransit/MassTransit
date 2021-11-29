namespace MassTransit
{
    using System;


    public interface IDispatchConfigurator<TContext>
    {
        void Pipe<T>(Action<IPipeConfigurator<T>> configurePipe)
            where T : class, PipeContext;
    }
}
