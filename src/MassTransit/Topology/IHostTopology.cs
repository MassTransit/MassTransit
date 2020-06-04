namespace MassTransit.Topology
{
    using System;


    public interface IHostTopology :
        IBusTopology
    {
        Uri HostAddress { get; }
    }
}
