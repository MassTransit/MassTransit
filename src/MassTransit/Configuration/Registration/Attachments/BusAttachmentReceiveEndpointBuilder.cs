namespace MassTransit.Registration.Attachments
{
    using System;
    using Builders;
    using Configuration;
    using Context;


    public class BusAttachmentReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _configuration;

        public BusAttachmentReceiveEndpointBuilder(IBusInstance busInstance, IReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _busInstance = busInstance;
            _configuration = configuration;
        }

        public ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new BusAttachmentReceiveEndpointContext(_busInstance, _configuration);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);

            return context;
        }


        class BusAttachmentReceiveEndpointContext :
            BaseReceiveEndpointContext
        {
            readonly IBusInstance _busInstance;

            public BusAttachmentReceiveEndpointContext(IBusInstance busInstance, IReceiveEndpointConfiguration configuration)
                : base(configuration)
            {
                _busInstance = busInstance;
            }

            protected override IPublishEndpointProvider CreatePublishEndpointProvider()
            {
                return _busInstance.Bus;
            }

            protected override ISendEndpointProvider CreateSendEndpointProvider()
            {
                return _busInstance.Bus;
            }

            protected override ISendTransportProvider CreateSendTransportProvider()
            {
                throw new NotSupportedException();
            }

            protected override IPublishTransportProvider CreatePublishTransportProvider()
            {
                throw new NotSupportedException();
            }
        }
    }
}
