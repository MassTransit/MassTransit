namespace MassTransit.AzureCosmos
{
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Internals;
    using Newtonsoft.Json;
    using Saga;
    using Serialization;


    public static class AzureCosmosSerializerExtensions
    {
        public static JsonSerializerSettings GetSagaRenameSettings<TSaga>()
            where TSaga : class, ISaga
        {
            var resolver = new PropertyRenameSerializerContractResolver();
            resolver.RenameProperty(typeof(TSaga), nameof(ISaga.CorrelationId), "id");

            return new JsonSerializerSettings { ContractResolver = resolver };
        }

        public static JsonSerializerOptions GetSerializerOptions<T>()
            where T : class, ISaga
        {
            var correlationId = MessageTypeCache<T>.Properties.Single(x => x.Name == nameof(ISaga.CorrelationId));

            if (correlationId.GetAttribute<JsonPropertyNameAttribute>().Any(x => x.Name == "id"))
                return SystemTextJsonMessageSerializer.Options;

            var options = new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options);
            options.PropertyNamingPolicy = new SagaRenamePropertyNamingPolicy(options.PropertyNamingPolicy);

            return options;
        }
    }
}
