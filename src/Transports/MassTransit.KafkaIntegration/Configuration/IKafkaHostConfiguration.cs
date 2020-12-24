namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public interface IKafkaHostConfiguration :
        ISpecification
    {
        IReadOnlyDictionary<string, string> Configuration { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }

        IKafkaRider Build(IBusInstance busInstance);
    }
}
