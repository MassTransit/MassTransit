namespace MassTransit.DependencyInjection.Testing
{
    using System;
    using System.Threading.Tasks;


    public class ContainerTestHarnessBusObserver :
        IBusObserver
    {
        readonly ContainerTestHarness _harness;

        public ContainerTestHarnessBusObserver(ContainerTestHarness harness)
        {
            _harness = harness;
        }

        public void PostCreate(IBus bus)
        {
            _harness.PostCreate(bus);
        }

        public void CreateFaulted(Exception exception)
        {
        }

        public Task PreStart(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task PostStop(IBus bus)
        {
            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
