namespace MassTransit.Azure.Cosmos
{
    using MassTransit.Saga;
    using Newtonsoft.Json;
    using Saga;


    public static class JsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings GetSagaRenameSettings<TSaga>()
            where TSaga : class, ISaga
        {
            var resolver = new PropertyRenameSerializerContractResolver();
            resolver.RenameProperty(typeof(TSaga), nameof(ISaga.CorrelationId), "id");

            return new JsonSerializerSettings {ContractResolver = resolver};
        }
    }
}
