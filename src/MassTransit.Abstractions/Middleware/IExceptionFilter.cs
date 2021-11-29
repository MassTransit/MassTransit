namespace MassTransit
{
    using System;


    /// <summary>
    /// Filter exceptions for policies that act based on an exception
    /// </summary>
    public interface IExceptionFilter :
        IProbeSite
    {
        /// <summary>
        /// Returns true if the exception matches the filter and the policy should
        /// be applied to the exception.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns>True if the exception matches the filter, otherwise false.</returns>
        bool Match(Exception exception);
    }
}
