namespace MassTransit.KafkaIntegration
{
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public interface IKafkaHostConfiguration :
        ISpecification
    {
        IClientContextSupervisor ClientContextSupervisor { get; }

        IKafkaRider Build(IBusInstance busInstance);
    }
}
