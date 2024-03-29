﻿namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ActivityExecutionFaultedException :
        ActivityExecutionException
    {
        public ActivityExecutionFaultedException()
            : this("The routing slip activity execution faulted with an unspecified exception")
        {
        }

        public ActivityExecutionFaultedException(string message)
            : base(message)
        {
        }

        public ActivityExecutionFaultedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected ActivityExecutionFaultedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
