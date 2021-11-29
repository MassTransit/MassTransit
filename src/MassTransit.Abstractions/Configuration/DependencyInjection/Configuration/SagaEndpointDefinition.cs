namespace MassTransit.Configuration
{
    public class SagaEndpointDefinition<TSaga> :
        SettingsEndpointDefinition<TSaga>
        where TSaga : class, ISaga
    {
        public SagaEndpointDefinition(IEndpointSettings<IEndpointDefinition<TSaga>> settings)
            : base(settings)
        {
        }

        protected override string FormatEndpointName(IEndpointNameFormatter formatter)
        {
            return formatter.Saga<TSaga>();
        }
    }
}
