namespace MassTransit.Registration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Riders;


    public interface IBusInstance
    {
        Type InstanceType { get; }

        IBus Bus { get; }
        IBusControl BusControl { get; }

        IHostConfiguration HostConfiguration { get; }
        void Add(IRider rider);

        Task Start(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
    }


    public interface IBusInstance<out TBus> :
        IBusInstance
        where TBus : IBus
    {
        TBus BusInstance { get; }
    }
}
