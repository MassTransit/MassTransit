namespace MassTransit.Configuration
{
    public class FutureEndpointDefinition<TFuture> :
        SettingsEndpointDefinition<TFuture>
        where TFuture : class
    {
        public FutureEndpointDefinition(IEndpointSettings<IEndpointDefinition<TFuture>> settings)
            : base(settings)
        {
        }

        protected override string FormatEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.Message<TFuture>();
        }
    }
}
