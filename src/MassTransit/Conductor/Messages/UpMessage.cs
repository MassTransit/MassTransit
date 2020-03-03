namespace MassTransit.Conductor.Messages
{
    using Contracts;


    public class UpMessage<T> :
        Up<T>
        where T : class
    {
        public UpMessage(ServiceInfo service, InstanceInfo instance, MessageInfo message)
        {
            Service = service;
            Instance = instance;
            Message = message;
        }

        public ServiceInfo Service { get; }
        public InstanceInfo Instance { get; }
        public MessageInfo Message { get; }
    }
}
