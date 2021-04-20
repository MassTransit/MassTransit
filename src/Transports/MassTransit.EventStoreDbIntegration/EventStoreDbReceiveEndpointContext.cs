using System;
using GreenPipes.Agents;
using MassTransit.Configuration;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.EventStoreDbIntegration.Exceptions;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Registration;
using MassTransit.Util;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbReceiveEndpointContext :
        BaseReceiveEndpointContext,
        IEventStoreDbSubscriptionContext
    {
        readonly IBusInstance _busInstance;
        readonly Recycle<ISubscriptionContextSupervisor> _contextSupervisor;

        public EventStoreDbReceiveEndpointContext(IEventStoreDbHostConfiguration hostConfiguration, IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration,
            SubscriptionSettings receiveSettings,
            IHeadersDeserializer headersDeserializer,
            CheckpointStoreFactory checkpointStoreFactory)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {

            _busInstance = busInstance;
            _contextSupervisor = new Recycle<ISubscriptionContextSupervisor>(() =>
                new SubscriptionContextSupervisor(hostConfiguration.ConnectionContextSupervisor, busInstance.HostConfiguration, receiveSettings,
                    headersDeserializer, checkpointStoreFactory));
        }

        public override void AddConsumeAgent(IAgent agent)
        {
            _contextSupervisor.Supervisor.AddConsumeAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return new EventStoreDbConnectionException(message, exception);
        }

        public ISubscriptionContextSupervisor ContextSupervisor => _contextSupervisor.Supervisor;

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            throw new NotSupportedException();
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            throw new NotSupportedException();
        }

        protected override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return _busInstance.Bus;
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            return _busInstance.Bus;
        }
    }
}
