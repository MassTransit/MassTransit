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

        protected JobAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
