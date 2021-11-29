namespace MassTransit.Util
{
    using System;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;


    public static class DisposeAsyncExtensions
    {
        /// <summary>
        /// Invoke the dispose callback, and then rethrow the exception
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="disposeCallback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="MassTransitException"></exception>
        public static ValueTask<T> DisposeAsync<T>(this Exception exception, Func<Task> disposeCallback)
        {
            var dispatchInfo = ExceptionDispatchInfo.Capture(exception.GetBaseException());

            async ValueTask<T> Faulted()
            {
                await disposeCallback().ConfigureAwait(false);

                dispatchInfo.Throw();

                throw new MassTransitException("DisposeAsync", exception);
            }

            return Faulted();
        }

        /// <summary>
        /// Invoke the dispose callback, and then rethrow the exception
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="disposeCallback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="MassTransitException"></exception>
        public static ValueTask<T> DisposeAsync<T>(this Exception exception, Func<ValueTask> disposeCallback)
        {
            var dispatchInfo = ExceptionDispatchInfo.Capture(exception.GetBaseException());

            async ValueTask<T> Faulted()
            {
                await disposeCallback().ConfigureAwait(false);

                dispatchInfo.Throw();

                throw new MassTransitException("DisposeAsync", exception);
            }

            return Faulted();
        }
    }
}
