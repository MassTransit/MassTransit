namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Attachments;
    using Configuration;


    public class TransportBusInstance :
        IBusInstance
    {
        readonly IList<IBusAttachment> _attachments = new List<IBusAttachment>();

        public TransportBusInstance(IBusControl busControl, IHostConfiguration hostConfiguration)
        {
            BusControl = busControl;
            HostConfiguration = hostConfiguration;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration { get; }

        public void Connect(IBusAttachment attachment)
        {
            _attachments.Add(attachment);
        }
    }
}
