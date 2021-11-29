namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Supports the asynchronous notification of a PipeContext becoming available (this is a future of a future, basically)
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    public interface IAsyncPipeContextHandle<TContext> :
        PipeContextHandle<TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Called when the PipeContext has been created and is available for use.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Created(TContext context);

        /// <summary>
        /// Called when the PipeContext creation was canceled
        /// </summary>
        Task CreateCanceled();

        /// <summary>
        /// Called when the PipeContext creation failed
        /// </summary>
        /// <param name="exception"></param>
        Task CreateFaulted(Exception exception);

        /// <summary>
        /// Called when the successfully created PipeContext becomes faulted, indicating that it
        /// should no longer be used.
        /// </summary>
        /// <param name="exception">The exception which occurred</param>
        Task Faulted(Exception exception);
    }
}
