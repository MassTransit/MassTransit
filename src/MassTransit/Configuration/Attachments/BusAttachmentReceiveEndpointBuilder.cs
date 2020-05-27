namespace MassTransit.Attachments
{
    using System;
    using Builders;
    using Configuration;
    using Context;
    using MassTransit.Registration;


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
            return new BusAttachmentEndpointReceiveContext(_busInstance, _configuration);
        }


        class BusAttachmentEndpointReceiveContext :
            BaseReceiveEndpointContext
        {
            readonly IBusInstance _busInstance;

            public BusAttachmentEndpointReceiveContext(IBusInstance busInstance, IReceiveEndpointConfiguration configuration)
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
