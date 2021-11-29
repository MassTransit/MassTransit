namespace MassTransit.Configuration
{
    using Courier;


    public class CompensateActivityEndpointDefinition<TActivity, TLog> :
        SettingsEndpointDefinition<ICompensateActivity<TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public CompensateActivityEndpointDefinition(IEndpointSettings<IEndpointDefinition<ICompensateActivity<TLog>>> settings)
            : base(settings)
        {
        }

        protected override string FormatEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.CompensateActivity<TActivity, TLog>();
        }
    }
}
