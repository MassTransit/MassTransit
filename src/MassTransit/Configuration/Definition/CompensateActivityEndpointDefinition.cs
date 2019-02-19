namespace MassTransit.Definition
{
    using Courier;


    public class CompensateActivityEndpointDefinition<TActivity, TLog> :
        SettingsEndpointDefinition<CompensateActivity<TLog>>
        where TActivity : class, CompensateActivity<TLog>
        where TLog : class
    {
        public CompensateActivityEndpointDefinition(IEndpointSettings<IEndpointDefinition<CompensateActivity<TLog>>> settings)
            : base(settings)
        {
        }

        protected override string FormatEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.CompensateActivity<TActivity, TLog>();
        }
    }
}
