namespace MassTransit
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Serialization;


    public static class HealthReportExtensions
    {
        public static string ToJsonString(this HealthReport result)
        {
            var healthResult = new JsonObject
            {
                ["status"] = result.Status.ToString(),
                ["results"] = new JsonObject(result.Entries.Select(entry => new KeyValuePair<string, JsonNode>(entry.Key,
                    new JsonObject
                    {
                        ["status"] = entry.Value.Status.ToString(),
                        ["description"] = entry.Value.Description,
                        ["data"] = JsonSerializer.SerializeToNode(entry.Value.Data, SystemTextJsonMessageSerializer.Options)
                    })))
            };

            var options = new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options)
            {
                WriteIndented = true,
            };

            return healthResult.ToJsonString(options);
        }
    }
}
