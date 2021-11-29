namespace MassTransit.Transports
{
    public interface IReceiveTransport :
        IReceiveObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IReceiveTransportObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// Start receiving on a transport, sending messages to the specified pipe.
        /// </summary>
        /// <returns></returns>
        ReceiveTransportHandle Start();
    }
}
