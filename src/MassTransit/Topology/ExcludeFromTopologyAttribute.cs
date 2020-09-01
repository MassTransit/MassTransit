namespace MassTransit.Topology
{
    using System;


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ExcludeFromTopologyAttribute :
        Attribute
    {
    }
}
