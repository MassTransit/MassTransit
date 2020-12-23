namespace MassTransit.KafkaIntegration.Specifications
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Registration;


    public class KafkaBusInstanceSpecification :
        IBusInstanceSpecification
    {
        readonly IKafkaHostConfiguration _hostConfiguration;

        public KafkaBusInstanceSpecification(IKafkaHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public void Configure(IBusInstance busInstance)
        {
            var rider = _hostConfiguration.Build(busInstance);
            //TODO: REMOVE THIS
            busInstance.HostConfiguration.Agent.Completed.ContinueWith(_ => _hostConfiguration.ClientContextSupervisor.Stop(),
                TaskContinuationOptions.ExecuteSynchronously);
            busInstance.Connect(rider);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _hostConfiguration.Validate();
        }
    }
}
