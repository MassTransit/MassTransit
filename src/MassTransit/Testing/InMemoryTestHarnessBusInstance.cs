namespace MassTransit.Testing
{
    using System;
    using Configuration;
    using Registration;
    using Riders;


    public class InMemoryTestHarnessBusInstance :
        IBusInstance
    {
        public InMemoryTestHarnessBusInstance(InMemoryTestHarness testHarness)
        {
            Harness = testHarness;
        }

        public InMemoryTestHarness Harness { get; }

        public Type InstanceType => typeof(InMemoryTestHarness);
        public IBus Bus => Harness.Bus;
        public IBusControl BusControl => Harness.BusControl;
        public IHostConfiguration HostConfiguration => Harness.HostConfiguration;

        public void Connect<TRider>(IRiderControl riderControl)
            where TRider : IRider
        {
            throw new NotSupportedException();
        }

        public TRider GetRider<TRider>()
            where TRider : IRider
        {
            throw new NotSupportedException();
        }
    }
}
