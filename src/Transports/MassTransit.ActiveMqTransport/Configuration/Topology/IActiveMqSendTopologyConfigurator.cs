namespace MassTransit
{
    using System;


    public interface IActiveMqSendTopologyConfigurator :
        ISendTopologyConfigurator,
        IActiveMqSendTopology
    {
        Action<IActiveMqQueueConfigurator> ConfigureErrorSettings { set; }

        Action<IActiveMqQueueConfigurator> ConfigureDeadLetterSettings { set; }
    }
}
