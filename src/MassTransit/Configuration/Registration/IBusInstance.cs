namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Attachments;
    using Configuration;


    public interface IBusInstance
    {
        IReadOnlyList<IBusAttachment> Attachments { get; }
        Type InstanceType { get; }

        IBus Bus { get; }
        IBusControl BusControl { get; }

        IHostConfiguration HostConfiguration { get; }

        void Connect(IBusAttachment attachment);
    }


    public interface IBusInstance<out TBus> :
        IBusInstance
        where TBus : IBus
    {
        TBus BusInstance { get; }
    }
}
