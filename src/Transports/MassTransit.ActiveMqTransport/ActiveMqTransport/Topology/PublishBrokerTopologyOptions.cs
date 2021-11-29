namespace MassTransit.ActiveMqTransport.Topology
{
    using System;


    [Flags]
    public enum PublishBrokerTopologyOptions
    {
        FlattenHierarchy = 0,
        MaintainHierarchy = 1
    }
}
