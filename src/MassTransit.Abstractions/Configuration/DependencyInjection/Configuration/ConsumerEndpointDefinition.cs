namespace MassTransit.Configuration
{
    public class ConsumerEndpointDefinition<TConsumer> :
        SettingsEndpointDefinition<TConsumer>
        where TConsumer : class, IConsumer
    {
        public ConsumerEndpointDefinition(IEndpointSettings<IEndpointDefinition<TConsumer>> settings)
            : base(settings)
        {
        }

        protected override string FormatEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.Consumer<TConsumer>();
        }
    }
}
