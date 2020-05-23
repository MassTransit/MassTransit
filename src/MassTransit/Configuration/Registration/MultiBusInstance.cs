namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Attachments;
    using Configuration;


    public class MultiBusInstance<TBus> :
        IBusInstance<TBus>
        where TBus : IBus
    {
        readonly List<IBusAttachment> _attachments = new List<IBusAttachment>();
        readonly IBusInstance _instance;

        public MultiBusInstance(TBus bus, IBusInstance instance)
        {
            _instance = instance;
            BusInstance = bus;
        }

        public IReadOnlyList<IBusAttachment> Attachments => _attachments;

        public Type InstanceType => typeof(TBus);
        public IBus Bus => BusInstance;
        public IBusControl BusControl => _instance.BusControl;
        public IHostConfiguration HostConfiguration => _instance.HostConfiguration;

        public void Connect(IBusAttachment attachment)
        {
            _attachments.Add(attachment);
        }

        public TBus BusInstance { get; }
    }
}
