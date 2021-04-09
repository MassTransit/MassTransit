using System;
using System.Collections.Generic;
using System.Text;
using MassTransit;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbProducerConfigurator :
        ISendObserverConnector,
        ISendPipelineConfigurator
    {
        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="serializerFactory">The factory to create the message serializer</param>
        void SetMessageSerializer(SerializerFactory serializerFactory);
    }
}
