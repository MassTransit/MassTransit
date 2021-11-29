namespace MassTransit.Configuration
{
    using Courier;


    public class ExecuteActivityEndpointDefinition<TActivity, TArguments> :
        SettingsEndpointDefinition<IExecuteActivity<TArguments>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        public ExecuteActivityEndpointDefinition(IEndpointSettings<IEndpointDefinition<IExecuteActivity<TArguments>>> settings)
            : base(settings)
        {
        }

        protected override string FormatEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.ExecuteActivity<TActivity, TArguments>();
        }
    }
}
