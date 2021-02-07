namespace MassTransit.Conductor.Orchestration
{
    using System;


    public interface BuildContext<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        void RequestEndpoint<T>(NextBuildContext<T, TResult, TInput> context, Uri inputAddress)
            where T : class;

        void Factory<T>(NextBuildContext<T, TResult, TInput> context, AsyncMessageFactory<TInput, T> factoryMethod)
            where T : class;

        void Result();

        NextBuildContext<T, TResult, TInput> Create<T>()
            where T : class;
    }
}
