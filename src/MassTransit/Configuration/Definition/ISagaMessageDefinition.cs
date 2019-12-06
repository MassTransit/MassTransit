namespace MassTransit.Definition
{
    using System;
    using Saga;


    public interface ISagaMessageDefinition<TSaga, TMessage> :
        ISagaMessageDefinition<TSaga>
        where TSaga : class, ISaga
        where TMessage : class
    {
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaMessageConfigurator<TSaga, TMessage>
            sagaConfigurator);
    }


    public interface ISagaMessageDefinition<TSaga> :
        ISagaMessageDefinition
        where TSaga : class, ISaga
    {
        void Configure(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TSaga> sagaConfigurator);
    }


    public interface ISagaMessageDefinition :
        IDefinition
    {
        /// <summary>
        /// The saga type
        /// </summary>
        Type SagaType { get; }

        /// <summary>
        /// The message type
        /// </summary>
        Type MessageType { get; }
    }
}
