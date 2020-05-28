namespace MassTransit.Riders
{
    using System;
    using Builders;
    using Configuration;
    using Context;
    using Registration;


    public class RiderReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _configuration;

        public RiderReceiveEndpointBuilder(IBusInstance busInstance, IReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _busInstance = busInstance;
            _configuration = configuration;
        }

        public ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new RiderReceiveEndpointContext(_busInstance, _configuration);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);

            return context;
        }


        class RiderReceiveEndpointContext :
            BaseReceiveEndpointContext
        {
            readonly IBusInstance _busInstance;

            public RiderReceiveEndpointContext(IBusInstance busInstance, IReceiveEndpointConfiguration configuration)
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
