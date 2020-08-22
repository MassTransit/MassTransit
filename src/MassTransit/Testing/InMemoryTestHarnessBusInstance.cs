namespace MassTransit.Testing
{
    using System;
    using Configuration;
    using GreenPipes;
    using Registration;
    using Riders;


    public class InMemoryTestHarnessBusInstance :
        IBusInstance
    {
        readonly RiderConnectable _riders;

        public InMemoryTestHarnessBusInstance(InMemoryTestHarness testHarness, RiderConnectable riders)
        {
            _riders = riders;
            Harness = testHarness;
        }

        public InMemoryTestHarness Harness { get; }

        public ConnectHandle ConnectRider(IRider rider)
        {
            return _riders.Connect(rider);
        }

        public Type InstanceType => typeof(InMemoryTestHarness);
        public IBus Bus => Harness.Bus;
        public IBusControl BusControl => Harness.BusControl;
        public IHostConfiguration HostConfiguration => Harness.HostConfiguration;

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            return _riders.Get<TRider>();
        }
    }
}
