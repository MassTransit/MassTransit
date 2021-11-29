namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public interface FutureCompleted
    {
        /// <summary>
        /// When the future was initially created
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// When the future was finally completed
        /// </summary>
        DateTime Completed { get; }
    }
}
