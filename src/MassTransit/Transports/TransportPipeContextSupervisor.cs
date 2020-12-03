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
        readonly ISupervisor _transportSupervisor;

        protected TransportPipeContextSupervisor(IAgent supervisor, IPipeContextFactory<T> factory)
            : base(factory)
        {
            _transportSupervisor = new Supervisor();

            supervisor.Completed.ContinueWith(_ => this.Stop(), TaskContinuationOptions.ExecuteSynchronously);
        }

        protected TransportPipeContextSupervisor(IPipeContextFactory<T> factory)
            : base(factory)
        {
            _transportSupervisor = new Supervisor();
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }

        public void AddAgent<TAgent>(TAgent agent)
            where TAgent : IAgent
        {
            _transportSupervisor.Add(agent);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await _transportSupervisor.Stop(context).ConfigureAwait(false);

            await base.StopSupervisor(context).ConfigureAwait(false);
        }
    }
}
