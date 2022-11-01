namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Middleware;


    public class TransportPipeContextSupervisor<T> :
        PipeContextSupervisor<T>,
        ITransportSupervisor<T>
        where T : class, PipeContext
    {
        readonly ISupervisor _consumeSupervisor;
        readonly ISupervisor _sendSupervisor;

        protected TransportPipeContextSupervisor(IPipeContextFactory<T> factory)
            : base(factory)
        {
            _consumeSupervisor = new Supervisor();
            _sendSupervisor = new Supervisor();
        }

        public CancellationToken ConsumeStopping => _consumeSupervisor.Stopping;
        public CancellationToken SendStopping => _sendSupervisor.Stopping;

        public void AddSendAgent<TAgent>(TAgent agent)
            where TAgent : IAgent
        {
            _sendSupervisor.Add(agent);
        }

        public void AddConsumeAgent<TAgent>(TAgent agent)
            where TAgent : IAgent
        {
            _consumeSupervisor.Add(agent);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await _consumeSupervisor.Stop(context).ConfigureAwait(false);

            await _sendSupervisor.Stop(context).ConfigureAwait(false);

            await base.StopSupervisor(context).ConfigureAwait(false);
        }
    }
}
