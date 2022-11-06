namespace MassTransit.AzureCosmos
{
    using System.Text.Json;


    public class SagaRenamePropertyNamingPolicy :
        JsonNamingPolicy
    {
        readonly JsonNamingPolicy _policy;

        public SagaRenamePropertyNamingPolicy(JsonNamingPolicy policy)
        {
            _policy = policy;
        }

        public JsonNamingPolicy Policy => _policy;

        public override string ConvertName(string name)
        {
            if (name.Equals(nameof(ISaga.CorrelationId)))
                return "id";

            return _policy?.ConvertName(name) ?? name;
        }
    }
}
