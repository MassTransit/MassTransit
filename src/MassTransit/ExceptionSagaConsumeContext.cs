namespace MassTransit
{
    using System;
    using Saga;


    public interface ExceptionSagaConsumeContext<T> :
        SagaConsumeContext<T>
        where T : class, ISaga
    {
        /// <summary>
        /// The exception that was thrown
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// The exception info, suitable for inclusion in a fault message
        /// </summary>
        ExceptionInfo ExceptionInfo { get; }
    }
}
