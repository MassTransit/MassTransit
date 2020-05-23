namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Attachments;
    using Configuration;


    public class DefaultBusInstance :
        IBusInstance
    {
        readonly IList<IBusAttachment> _attachments = new List<IBusAttachment>();

        public DefaultBusInstance(IBusControl busControl)
        {
            BusControl = busControl;
        }

        public Type InstanceType => typeof(IBus);
        public IBus Bus => BusControl;
        public IBusControl BusControl { get; }

        public IHostConfiguration HostConfiguration => default;

        public void Connect(IBusAttachment attachment)
        {
            _attachments.Add(attachment);
        }
    }
}
