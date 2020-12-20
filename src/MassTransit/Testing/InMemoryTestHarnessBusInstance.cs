namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using GreenPipes;
    using Registration;
    using Riders;


    public class InMemoryTestHarnessBusInstance :
        IBusInstance
    {
        readonly List<IRider> _riders;

        public InMemoryTestHarnessBusInstance(InMemoryTestHarness testHarness)
        {
            _riders = new List<IRider>();
            Harness = testHarness;
        }

        public InMemoryTestHarness Harness { get; }

        public void ConnectRider(IRider rider)
        {
            _riders.Add(rider);
        }

        public Type InstanceType => typeof(InMemoryTestHarness);
        public IBus Bus => Harness.Bus;
        public IBusControl BusControl => Harness.BusControl;
        public IHostConfiguration HostConfiguration => Harness.HostConfiguration;

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return _riders.OfType<TRider>().FirstOrDefault();
        }
    }
}
