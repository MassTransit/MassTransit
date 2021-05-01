namespace MassTransit.JobService.Components
{
    using System;


    public interface IServiceInstance
    {
        Guid InstanceId { get; }
        string InstanceName { get; }
    }
}
