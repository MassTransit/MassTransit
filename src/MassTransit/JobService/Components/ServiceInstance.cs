namespace MassTransit.JobService.Components
{
    using System;
    using Util;


    public class ServiceInstance :
        IServiceInstance
    {
        public ServiceInstance()
        {
            var instanceId = NewId.Next();

            InstanceId = instanceId.ToGuid();
            InstanceName = instanceId.ToString(FormatUtil.Formatter);
        }

        public Guid InstanceId { get; }
        public string InstanceName { get; }
    }
}
