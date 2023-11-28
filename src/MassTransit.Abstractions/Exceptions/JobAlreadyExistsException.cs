namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class JobAlreadyExistsException :
        MassTransitException
    {
        public JobAlreadyExistsException()
        {
        }

        public JobAlreadyExistsException(Guid jobId)
            : base($"The job already exists in the roster: {jobId}")
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected JobAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
