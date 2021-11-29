namespace MassTransit.Internals
{
    using System;
    using System.Runtime.ExceptionServices;


    public static class ExceptionExtensions
    {
        /// <summary>
        /// Rethrow the exception with the call stack of the original exception
        /// </summary>
        /// <param name="exception"></param>
        public static void Rethrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}
