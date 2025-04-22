namespace MassTransit;

using System;
using System.Runtime.Serialization;


[Serializable]
public class JobServiceStoppingException :
    MassTransitException
{
    public JobServiceStoppingException()
    {
    }

    public JobServiceStoppingException(Guid jobId)
        : base($"The job service is stopping, job cannot be started: {jobId}")
    {
    }

#if NET8_0_OR_GREATER
    [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
    protected JobServiceStoppingException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
