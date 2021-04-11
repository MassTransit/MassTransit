using MassTransit.Configuration;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProcessorContextSupervisor :
        TransportPipeContextSupervisor<ProcessorContext>,
        IProcessorContextSupervisor
    {

        public ProcessorContextSupervisor(IClientContextSupervisor supervisor, IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings)
            : base(new ProcessorContextFactory(supervisor, hostConfiguration, receiveSettings, null))
        {
            supervisor.AddConsumeAgent(this);
        }
    }
}
