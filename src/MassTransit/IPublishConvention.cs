namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IPublishConvention
    {
        Task<ISendEndpoint> GetSendEndpoint<T>(T message);

        Task<ISendEndpoint> GetSendEndpoint(object message, Type messageType);
    }
}
