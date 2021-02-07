namespace MassTransit.TestComponents.ForkJoint.Contracts
{
    using System;
    using MassTransit;


    public interface FutureFaulted
    {
        /// <summary>
        /// When the future was initially created
        /// </summary>
        DateTime? Created { get; }

        /// <summary>
        /// When the future faulted
        /// </summary>
        DateTime? Faulted { get; }

        /// <summary>
        /// The exception related to the fault
        /// </summary>
        ExceptionInfo[] Exceptions { get; }
    }
}