namespace MassTransit.Registration
{
    using Courier;


    public class CompensateActivityEndpointRegistrationConfigurator<TActivity, TLog> :
        EndpointRegistrationConfigurator<ICompensateActivity<TLog>>,
        ICompensateActivityEndpointRegistrationConfigurator
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public CompensateActivityEndpointRegistrationConfigurator()
        {
            ConfigureConsumeTopology = false;
        }
    }
}
