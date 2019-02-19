namespace MassTransit.Definition
{
    using Courier;


    public class ExecuteActivityEndpointDefinition<TActivity, TArguments> :
        SettingsEndpointDefinition<ExecuteActivity<TArguments>>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        public ExecuteActivityEndpointDefinition(IEndpointSettings<IEndpointDefinition<ExecuteActivity<TArguments>>> settings)
            : base(settings)
        {
        }

        protected override string FormatEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.ExecuteActivity<TActivity, TArguments>();
        }
    }
}
