namespace MassTransit.Azure.ServiceBus.Core.Configurators
{
    using System;


    public interface IReceiverConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Set the input address of the receiver
        /// </summary>
        new Uri InputAddress { get; set; }
    }
}
