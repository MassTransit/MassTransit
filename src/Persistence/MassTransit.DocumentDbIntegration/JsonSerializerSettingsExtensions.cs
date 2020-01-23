namespace MassTransit.DocumentDbIntegration
{
    using Newtonsoft.Json;
    using Saga;


    public static class JsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings GetSagaRenameSettings<TSaga>()
            where TSaga : class, IVersionedSaga
        {
            var resolver = new PropertyRenameSerializerContractResolver();
            resolver.RenameProperty(typeof(TSaga), "CorrelationId", "id");
            resolver.RenameProperty(typeof(TSaga), "ETag", "_etag");

            return new JsonSerializerSettings {ContractResolver = resolver};
        }
    }
}
