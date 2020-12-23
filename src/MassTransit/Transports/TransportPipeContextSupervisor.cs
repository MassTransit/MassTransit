namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;


    public class TransportPipeContextSupervisor<T> :
        PipeContextSupervisor<T>,
        ITransportSupervisor<T>
        where T : class, PipeContext
    {
        readonly ISupervisor _consumeSupervisor;
        readonly ISupervisor _sendSupervisor;

        protected TransportPipeContextSupervisor(IAgent supervisor, IPipeContextFactory<T> factory)
            : base(factory)
        {
            _consumeSupervisor = new Supervisor();
            _sendSupervisor = new Supervisor();

            supervisor.Completed.ContinueWith(_ => this.Stop(), TaskContinuationOptions.ExecuteSynchronously);
        }

        protected TransportPipeContextSupervisor(IPipeContextFactory<T> factory)
            : base(factory)
        {
            _consumeSupervisor = new Supervisor();
            _sendSupervisor = new Supervisor();
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }

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
