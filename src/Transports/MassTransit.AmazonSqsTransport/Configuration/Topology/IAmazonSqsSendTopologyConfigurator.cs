namespace MassTransit;

using System;


public interface IAmazonSqsSendTopologyConfigurator :
    ISendTopologyConfigurator,
    IAmazonSqsSendTopology
{
    Action<IAmazonSqsQueueConfigurator>? ConfigureErrorSettings { set; }
    Action<IAmazonSqsQueueConfigurator>? ConfigureDeadLetterSettings { set; }
}
