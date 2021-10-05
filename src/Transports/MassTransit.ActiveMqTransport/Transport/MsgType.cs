namespace MassTransit.ActiveMqTransport.Transport
{
    /// <summary>
    /// Use to define which JMS message type in infrastructure sending message <see cref="ActiveMqSendConfigurator"/>
    /// </summary>
    public enum MsgType
    {
        Binary,
        Text,
        Object
    }
}
