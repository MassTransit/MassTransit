namespace MassTransit.Riders
{
    using System.Threading;
    using GreenPipes.Agents;


    public interface IRiderCollection :
        IAgent
    {
        IRider Get(string name);

        void Add(string name, IRiderControl rider);

        HostRiderHandle[] StartRiders(CancellationToken cancellationToken = default);

        HostRiderHandle StartRider(string name, CancellationToken cancellationToken = default);
    }
}
