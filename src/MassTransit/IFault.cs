namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public interface IFault
    {
        /// <summary>
        /// The type of fault that occurred
        /// </summary>
        string FaultType { get; }

        /// <summary>
        /// Messages associated with the exception
        /// </summary>
        List<string> Messages { get; set; }

        /// <summary>
        /// When the exception occurred
        /// </summary>
        DateTime OccurredAt { get; set; }

        /// <summary>
        /// A stack trace related to the exception
        /// </summary>
        List<string> StackTrace { get; set; }
    }
}
