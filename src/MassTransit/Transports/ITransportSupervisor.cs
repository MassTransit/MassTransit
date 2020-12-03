namespace MassTransit.Transports
{
    using GreenPipes;
    using GreenPipes.Agents;


    public interface ITransportSupervisor<out T> :
        ISupervisor<T>
        where T : class, PipeContext
    {
        void AddAgent<TAgent>(TAgent agent)
            where TAgent : IAgent;
    }
}
