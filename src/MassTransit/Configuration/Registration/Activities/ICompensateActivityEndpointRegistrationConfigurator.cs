namespace MassTransit.Registration
{
    using Courier;


    public interface ICompensateActivityEndpointRegistrationConfigurator<TActivity, TLog> :
        IEndpointRegistrationConfigurator
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
    }
}
